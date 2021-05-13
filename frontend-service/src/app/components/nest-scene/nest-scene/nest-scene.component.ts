import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Guid } from 'guid-typescript';
import { NgxDropzoneChangeEvent } from 'ngx-dropzone';
import {
  NestConfig,
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

  private nestConfig: NestConfig = {} as NestConfig;
  private binId = Guid.create();

  public constructor(
    private engineService: EngineService,
    private nestService: NestService
  ) {}

  public ngOnInit(): void {
    this.engineService.createScene(this.rendererCanvas, this.binId);
    this.engineService.render();
  }

  public onSelect(event: NgxDropzoneChangeEvent): void {
    event.addedFiles.forEach((x) => this.engineService.loadPolygon(x));
    this.files.push(...event.addedFiles);
  }

  public onRemove(event: File): void {
    this.files.splice(this.files.indexOf(event), 1);
  }

  public async nest(): Promise<void> {
    console.log('Nesting started...');
    const sceneObjects = this.engineService.getObjects();
    this.engineService.setToDefaults(sceneObjects);
    const sceneBin = sceneObjects.find((x) => x.uuid === this.binId.toString());
    if (!sceneBin) {
      return;
    }
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
    const nestResult = await this.nestService.nest(nestData, this.nestConfig);

    console.log('Applying placements...');
    this.placeObjects(nestResult.placements);

    console.log('Nesting finished.');
  }

  private placeObjects(placements: NestObjectPlacement[]): void {
    placements.forEach((x) => {
      this.engineService.move(x.id, x.translationPoint);
      this.engineService.rotate(x.id, x.rotation, x.translationPoint);
    });
  }
}
