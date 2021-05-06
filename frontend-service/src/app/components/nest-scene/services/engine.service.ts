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
  private dragging = false;
  private sensitivity = 0.05;

  public constructor() {}

  public ngOnDestroy(): void {
    if (this.frameId !== null) {
      cancelAnimationFrame(this.frameId);
    }
  }

  public createScene(canvas: ElementRef<HTMLCanvasElement>): void {
    // The first step is to get the reference of the canvas element from our HTML document
    this.canvas = canvas.nativeElement;

    this.canvas.addEventListener('wheel', (event) => this.zoom(event), {
      passive: false,
    });
    document.addEventListener(
      'mousedown',
      (event) => this.onDocumentMouseDown(event),
      {
        passive: false,
      }
    );
    document.addEventListener(
      'mousemove',
      (event) => this.onDocumentMouseMove(event),
      {
        passive: false,
      }
    );
    document.addEventListener(
      'mouseup',
      (event) => this.onDocumentMouseUp(event),
      {
        passive: false,
      }
    );

    this.renderer = new THREE.WebGLRenderer({
      canvas: this.canvas,
      alpha: true, // transparent background
      antialias: true, // smooth edges
    });

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
    points.push(new THREE.Vector3(-10, -10, 0));
    points.push(new THREE.Vector3(10, -10, 0));
    points.push(new THREE.Vector3(10, 10, 0));
    points.push(new THREE.Vector3(-10, 10, 0));
    this.addPolygonFromPoints(points, 0x0000ff);
  }

  public render(): void {
    this.renderer.render(this.scene, this.camera);
    this.animate();
  }

  public addPolygonFromPoints(
    points: THREE.Vector3[],
    color: THREE.Color | string | number,
    closed: boolean = false
  ): void {
    if (points.length === 0) {
      return;
    }

    if (!closed) {
      points.push(points[0]);
    }

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    this.addPolygonFromGeometry(geometry, color);
  }

  public addPolygonFromGeometry(
    geometry: THREE.BufferGeometry,
    color: THREE.Color | string | number
  ): void {
    const material = new THREE.LineBasicMaterial({ color });

    const line = new THREE.Line(geometry, material);
    this.scene.add(line);
  }

  private numberArrayToVectors(array: number[]): THREE.Vector3[] {
    const vectors = [];
    for (let i = 0; i < array.length; i += 3) {
      const vector = new THREE.Vector3(array[i], array[i + 1], array[i + 2]);
      vectors.push(vector);
    }
    return vectors;
  }

  public loadPolygon(file: File): void {
    const reader = new FileReader();

    reader.onload = () => {
      if (!reader.result) {
        return;
      }
      const obj = this.fbxLoader.parse(reader.result, '');
      console.log(obj);
      obj.children
        .filter((x) => x instanceof THREE.Mesh)
        .forEach((x) => {
          const points = this.numberArrayToVectors(
            Array.from((x as THREE.Mesh).geometry.attributes.position.array)
          );
          this.addPolygonFromPoints(points, 0xff0000, true);
        });
    };

    reader.readAsArrayBuffer(file);
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
    this.resizeCanvasToDisplaySize();
    this.renderer.render(this.scene, this.camera);
    requestAnimationFrame(this.animate.bind(this));
  }

  private zoom(event: WheelEvent): void {
    event.preventDefault();

    this.camera.position.z += event.deltaY / 20;
  }

  private onDocumentMouseDown(event: MouseEvent): void {
    event.preventDefault();

    this.dragging = true;
  }

  private onDocumentMouseUp(event: MouseEvent): void {
    this.dragging = false;
  }

  private onDocumentMouseMove(event: MouseEvent): void {
    if (!this.dragging) {
      return;
    }

    this.camera.position.y += event.movementY * this.sensitivity;
    this.camera.position.x -= event.movementX * this.sensitivity;
  }
}
