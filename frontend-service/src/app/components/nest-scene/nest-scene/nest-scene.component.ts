import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { WebGLService } from '../services/web-gl.service';

@Component({
  selector: 'app-nest-scene',
  templateUrl: './nest-scene.component.html',
  styleUrls: ['./nest-scene.component.scss'],
})
export class NestSceneComponent implements OnInit, AfterViewInit {
  @ViewChild('sceneCanvas') private canvas = {} as ElementRef<HTMLCanvasElement>;
  constructor(private webglService: WebGLService) {}
  ngOnInit(): void {}

  ngAfterViewInit(): void {
    if (!this.canvas) {
      alert('canvas not supplied! cannot bind WebGL context!');
      return;
    }
    this.webglService.initialiseWebGLContext(this.canvas.nativeElement);
  }
}
