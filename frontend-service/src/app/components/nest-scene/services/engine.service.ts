import { ElementRef, Injectable, OnDestroy } from '@angular/core';
import * as THREE from 'three';
import { FBXLoader } from 'three/examples/jsm/loaders/FBXLoader';

@Injectable()
export class EngineService implements OnDestroy {
  private canvas: HTMLCanvasElement = {} as HTMLCanvasElement;
  private renderer: THREE.WebGLRenderer = {} as THREE.WebGLRenderer;
  private fbxLoader: FBXLoader = {} as FBXLoader;
  private camera: THREE.PerspectiveCamera = {} as THREE.PerspectiveCamera;
  private scene: THREE.Scene = {} as THREE.Scene;
  private light: THREE.AmbientLight = {} as THREE.AmbientLight;

  private frameId: number | null = null;

  public constructor() {}

  public ngOnDestroy(): void {
    if (this.frameId !== null) {
      cancelAnimationFrame(this.frameId);
    }
  }

  public createScene(canvas: ElementRef<HTMLCanvasElement>): void {
    // The first step is to get the reference of the canvas element from our HTML document
    this.canvas = canvas.nativeElement;

    this.renderer = new THREE.WebGLRenderer({
      canvas: this.canvas,
      alpha: true, // transparent background
      antialias: true, // smooth edges
    });
    this.renderer.setSize(window.innerWidth, window.innerHeight);

    this.fbxLoader = new FBXLoader();

    // create the scene
    this.scene = new THREE.Scene();

    this.camera = new THREE.PerspectiveCamera(75, 2, 0.1, 1000);
    this.camera.position.z = 50;
    this.scene.add(this.camera);

    // soft white light
    this.light = new THREE.AmbientLight(0x404040);
    this.light.position.z = 10;
    this.scene.add(this.light);

    // test polygon
    const points = [];
    points.push(new THREE.Vector2(-10, 0));
    points.push(new THREE.Vector2(0, 10));
    points.push(new THREE.Vector2(10, 0));
    points.push(new THREE.Vector2(-10, 0));
    this.addPolygon(points, 0x0000ff);
  }

  public render(): void {
    this.renderer.render(this.scene, this.camera);
    this.animate();
  }

  public addPolygon(
    points: THREE.Vector2[],
    color: THREE.Color | string | number
  ): void {
    if (points.length === 0) {
      return;
    }

    points.push(points[0]);

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({ color });

    const line = new THREE.Line(geometry, material);
    this.scene.add(line);
  }

  public loadPolygon(path: string): void {
    this.fbxLoader.load(
      path,
      (obj) => {
        this.scene.add(obj);
      },
      (xhr) => {
        console.log((xhr.loaded / xhr.total) * 100 + '% loaded');
      },
      (error) => {
        console.log(error);
      }
    );
  }

  private resizeCanvasToDisplaySize(): boolean {
    const width = this.canvas.clientWidth;
    const height = this.canvas.clientHeight;
    const resizeNeeded =
      this.canvas.width !== width || this.canvas.height !== height;
    if (resizeNeeded) {
      // you must pass false here or three.js sadly fights the browser
      this.renderer.setSize(width, height, false);
      this.camera.aspect = width / height;
      this.camera.updateProjectionMatrix();
    }
    return resizeNeeded;
  }

  private animate(): void {
    const resizeNeeded = this.resizeCanvasToDisplaySize();
    if (!resizeNeeded) {
      return;
    }

    this.renderer.render(this.scene, this.camera);
    requestAnimationFrame(this.animate.bind(this));
  }
}
