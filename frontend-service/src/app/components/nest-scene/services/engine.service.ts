import * as THREE from 'three';
import {ElementRef, Injectable, OnDestroy} from '@angular/core';

@Injectable()
export class EngineService implements OnDestroy {
  private canvas: HTMLCanvasElement = {} as HTMLCanvasElement;
  private renderer: THREE.WebGLRenderer = {} as THREE.WebGLRenderer;
  private camera: THREE.PerspectiveCamera = {} as THREE.PerspectiveCamera;
  private scene: THREE.Scene = {} as THREE.Scene;
  private light: THREE.AmbientLight = {} as THREE.AmbientLight;

  private frameId: number | null = null;

  public constructor() {}

  public ngOnDestroy(): void {
    if (this.frameId != null) {
      cancelAnimationFrame(this.frameId);
    }
  }

  public createScene(canvas: ElementRef<HTMLCanvasElement>): void {
    // The first step is to get the reference of the canvas element from our HTML document
    this.canvas = canvas.nativeElement;

    this.renderer = new THREE.WebGLRenderer({
      canvas: this.canvas,
      alpha: true,    // transparent background
      antialias: true // smooth edges
    });
    this.renderer.setSize(window.innerWidth, window.innerHeight);

    // create the scene
    this.scene = new THREE.Scene();

    this.camera = new THREE.PerspectiveCamera(
      75, window.innerWidth / window.innerHeight, 0.1, 1000
    );
    this.camera.position.z = 50;
    this.scene.add(this.camera);

    // soft white light
    this.light = new THREE.AmbientLight(0x404040);
    this.light.position.z = 10;
    this.scene.add(this.light);

    const points = [];
    points.push(new THREE.Vector2(- 10, 0));
    points.push(new THREE.Vector2(0, 10));
    points.push(new THREE.Vector2(10, 0));
    points.push(new THREE.Vector2(-10, 0));

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color: 0x0000ff });

    const line = new THREE.Line(geometry, material);
    this.scene.add(line);

  }

  public render(): void {
    this.renderer.render(this.scene, this.camera);
  }
}
