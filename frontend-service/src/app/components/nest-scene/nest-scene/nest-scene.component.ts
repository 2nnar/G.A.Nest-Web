import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as FileSaver from 'file-saver';
import { Guid } from 'guid-typescript';
import { NgxDropzoneChangeEvent } from 'ngx-dropzone';
import {
  GCodeData,
  NestData,
  NestObjectPlacement,
  NestObjectType,
  NestPolygon,
} from 'src/app/back-models/nest.model';
import { GraphicsUtils } from 'src/app/utils/graphics.utils';
import { EngineService } from '../services/engine.service';
import { NestService } from '../services/nest.service';

@Component({
  selector: 'app-nest-scene',
  templateUrl: './nest-scene.component.html',
  styleUrls: ['./nest-scene.component.scss'],
})
export class NestSceneComponent implements OnInit {
  @ViewChild('rendererCanvas', { static: true })
  public rendererCanvas: ElementRef<HTMLCanvasElement> = {} as ElementRef<HTMLCanvasElement>;

  public files: File[] = [];
  public nestConfigControl: FormGroup = {} as FormGroup;
  public isNesting = false;
  public binWidth = 60;
  public binLength = 60;

  private binId = Guid.create();

  public constructor(
    private engineService: EngineService,
    private nestService: NestService,
    private formBuilder: FormBuilder
  ) {}

  public ngOnInit(): void {
    this.engineService.createScene(
      this.rendererCanvas,
      this.binId,
      this.binWidth,
      this.binLength
    );
    this.engineService.render();

    const formData = {
      tolerance: [3, [Validators.min(0)]],
      cutThickness: [0, [Validators.min(0)]],
      curveApproximation: [0.05, [Validators.min(0), Validators.max(1)]],
      rotationStep: [90, [Validators.min(0), Validators.max(360)]],
      iterationsCount: [1, [Validators.min(1)]],
      populationSize: [10, [Validators.min(2)]],
      mutationRate: [0.1, [Validators.min(0), Validators.max(1)]],
      holesUsing: [false],
    };

    this.nestConfigControl = this.formBuilder.group(formData);
  }

  public onSelect(event: NgxDropzoneChangeEvent): void {
    event.addedFiles.forEach((x) => this.engineService.loadPolygon(x));
    this.files.push(...event.addedFiles);
  }

  public onRemove(event: File): void {
    this.files.splice(this.files.indexOf(event), 1);
  }

  public updateBin(): void {
    this.engineService.updateRectangle(
      this.binId.toString(),
      this.binLength,
      this.binWidth
    );
  }

  public async nest(): Promise<void> {
    console.log('Nesting started...');
    this.isNesting = true;
    const nestConfig = this.nestConfigControl.getRawValue();
    const sceneObjects = this.engineService.getObjects();
    const sceneBin = sceneObjects.find((x) => x.uuid === this.binId.toString());
    if (!sceneBin) {
      alert('Bin is not found!');
      return;
    }
    this.engineService.setToDefaults([sceneBin]);

    const nestBin: NestPolygon = {
      id: sceneBin.uuid,
      type: 'Polygon' as NestObjectType,
      vertices: GraphicsUtils.numberArrayToPoints(
        Array.from(sceneBin.geometry.attributes.position.array)
      ),
    };
    const nestObjects: NestPolygon[] = sceneObjects
      .filter((x) => x.uuid !== this.binId.toString())
      .map((x) => {
        return {
          id: x.uuid,
          type: 'Polygon' as NestObjectType,
          vertices: GraphicsUtils.numberArrayToPoints(
            Array.from(x.geometry.attributes.position.array)
          ),
        };
      });

    const nestData: NestData = {
      bin: nestBin,
      objects: nestObjects,
    };
    const nestResult = await this.nestService.nest(nestData, nestConfig);

    console.log('Applying placements...');
    this.placeObjects(nestResult.placements);

    console.log('Nesting finished.');
    this.isNesting = false;
  }

  public async getGCode(): Promise<void> {
    const sceneObjects = this.engineService.getObjects();
    const nestObjects: NestPolygon[] = sceneObjects
      .filter((x) => x.uuid !== this.binId.toString())
      .map((x) => {
        return {
          id: x.uuid,
          type: 'Polygon' as NestObjectType,
          vertices: GraphicsUtils.numberArrayToPoints(
            Array.from(x.geometry.attributes.position.array)
          ),
        };
      });

    const gCodeData: GCodeData = {
      objects: nestObjects,
    };
    const gCodeResult = await this.nestService.getGCode(gCodeData);
    const blob = new Blob([gCodeResult.commands.join('\n')], {
      type: 'text/x.gcode',
    });
    FileSaver.saveAs(blob, 'result.gcode');
  }

  private placeObjects(placements: NestObjectPlacement[]): void {
    placements.forEach((x) => {
      this.engineService.move(x.id, x.translationPoint);
      this.engineService.rotate(x.id, x.rotation, x.translationPoint);
    });
  }
}
