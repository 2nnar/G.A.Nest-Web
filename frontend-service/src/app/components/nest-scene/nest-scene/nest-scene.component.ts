import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgxDropzoneChangeEvent } from 'ngx-dropzone';
import { EngineService } from '../services/engine.service';

@Component({
  selector: 'app-nest-scene',
  templateUrl: './nest-scene.component.html',
  styleUrls: ['./nest-scene.component.scss'],
})
export class NestSceneComponent implements OnInit {
  @ViewChild('rendererCanvas', { static: true })
  public rendererCanvas: ElementRef<HTMLCanvasElement> = {} as ElementRef<HTMLCanvasElement>;

  public files: File[] = [];

  public constructor(private engineService: EngineService) {}

  public ngOnInit(): void {
    this.engineService.createScene(this.rendererCanvas);
    this.engineService.render();
  }

  public onSelect(event: NgxDropzoneChangeEvent): void {
    event.addedFiles.forEach((x) => this.engineService.loadPolygon(x));
    this.files.push(...event.addedFiles);
  }

  public onRemove(event: File): void {
    this.files.splice(this.files.indexOf(event), 1);
  }
}
