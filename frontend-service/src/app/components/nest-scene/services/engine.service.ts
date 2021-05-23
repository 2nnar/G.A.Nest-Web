import { ElementRef, Injectable, OnDestroy } from '@angular/core';
import { Guid } from 'guid-typescript';
import { GraphicsUtils } from 'src/app/utils/graphics.utils';
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
  private raycaster: THREE.Raycaster = {} as THREE.Raycaster;
  private workspacePlane: THREE.Mesh = {} as THREE.Mesh;

  private frameId: number | null = null;
  private dragging = false;
  private pickedObject: THREE.Line | null = null;
  private sensitivity = 0.05;

  public constructor() {}

  public ngOnDestroy(): void {
    if (this.frameId !== null) {
      cancelAnimationFrame(this.frameId);
    }
  }

  public getObjects(): THREE.Line[] {
    const meshes: THREE.Line[] = [];
    this.scene.traverse((x) => {
      if (x instanceof THREE.Line) {
        meshes.push(x);
      }
    });
    return meshes;
  }

  public setToDefaults(objects: THREE.Object3D[]): void {
    objects.forEach((x) => {
      x.position.set(0, 0, 0);
      x.rotation.set(0, 0, 0);
    });
  }

  public move(id: string, point: { x: number; y: number; z: number }): void {
    const obj = this.scene.getObjectByProperty('uuid', id) as THREE.Line;
    obj?.position.add(new THREE.Vector3(point.x, point.y, point.z));

    this.updateGeometry(obj);
  }

  public rotate(
    id: string,
    angle: number,
    point: { x: number; y: number; z: number }
  ): void {
    const vector = new THREE.Vector3(point.x, point.y, point.z);
    const axis = new THREE.Vector3(0, 0, 1);
    const obj = this.scene.getObjectByProperty('uuid', id) as THREE.Line;
    obj?.position.sub(vector);
    obj?.position.applyAxisAngle(axis, angle);
    obj?.position.add(vector);
    obj?.rotateOnAxis(axis, angle);

    this.updateGeometry(obj);
  }

  public updateRectangle(id: string, length: number, width: number): void {
    const obj = this.scene.getObjectByProperty('uuid', id);
    const line = obj as THREE.Line;
    if (!line) {
      return;
    }

    if (line.geometry.attributes.position.count !== 5) {
      return;
    }

    const halfLength = length / 2;
    const halfWidth = width / 2;

    const points = [];
    points.push(new THREE.Vector3(-halfLength, -halfWidth, 0));
    points.push(new THREE.Vector3(halfLength, -halfWidth, 0));
    points.push(new THREE.Vector3(halfLength, halfWidth, 0));
    points.push(new THREE.Vector3(-halfLength, halfWidth, 0));
    points.push(new THREE.Vector3(-halfLength, -halfWidth, 0));

    line.geometry.setFromPoints(points);
    this.render();
  }

  public createScene(
    canvas: ElementRef<HTMLCanvasElement>,
    binId: Guid,
    binWidth: number,
    binLength: number
  ): void {
    // The first step is to get the reference of the canvas element from our HTML document
    this.canvas = canvas.nativeElement;

    this.canvas.addEventListener('wheel', (event) => this.onWheel(event), {
      passive: false,
    });
    this.canvas.addEventListener(
      'mousedown',
      (event) => this.onMouseDown(event),
      {
        passive: false,
      }
    );
    this.canvas.addEventListener(
      'mousemove',
      (event) => this.onMouseMove(event),
      {
        passive: false,
      }
    );
    this.canvas.addEventListener('mouseup', (event) => this.onMouseUp(event), {
      passive: false,
    });
    window.addEventListener('keydown', (event) => this.onKeyDown(event), {
      passive: false,
    });

    this.renderer = new THREE.WebGLRenderer({
      canvas: this.canvas,
      alpha: true, // transparent background
      antialias: true, // smooth edges
    });

    this.fbxLoader = new FBXLoader();

    const planeGeometry = new THREE.PlaneGeometry(
      this.canvas.width,
      this.canvas.height
    );
    const planeMaterial = new THREE.MeshBasicMaterial({ color: 0xcccccc });
    this.workspacePlane = new THREE.Mesh(planeGeometry, planeMaterial);
    this.raycaster = new THREE.Raycaster();

    // create the scene
    this.scene = new THREE.Scene();

    // this.scene.add(new THREE.AxesHelper(100));

    const halfWidth = binWidth / 2;
    const halfLength = binLength / 2;

    this.camera = new THREE.PerspectiveCamera(75, 2, 0.1, 1000);
    this.camera.position.x = halfLength;
    this.camera.position.y = halfWidth;
    this.camera.position.z = 50;
    this.scene.add(this.camera);

    // soft white light
    this.light = new THREE.AmbientLight(0x404040);
    this.light.position.z = 10;
    this.scene.add(this.light);

    // bin polygon
    const points = [];
    points.push(new THREE.Vector3(0, 0, 0));
    points.push(new THREE.Vector3(binLength, 0, 0));
    points.push(new THREE.Vector3(binLength, binWidth, 0));
    points.push(new THREE.Vector3(0, binWidth, 0));
    this.addPolygonFromPoints(points, 'black', binId.toString());

    const points1 = [];
    points1.push(new THREE.Vector3(5, 5, 0));
    points1.push(new THREE.Vector3(25, 5, 0));
    points1.push(new THREE.Vector3(25, 25, 0));
    points1.push(new THREE.Vector3(5, 25, 0));
    this.addPolygonFromPoints(points1, 0x0000ff, Guid.create().toString());

    const points2 = [];
    points2.push(new THREE.Vector3(35, 35, 0));
    points2.push(new THREE.Vector3(55, 35, 0));
    points2.push(new THREE.Vector3(55, 55, 0));
    points2.push(new THREE.Vector3(35, 55, 0));
    this.addPolygonFromPoints(points2, 0x0000ff, Guid.create().toString());
  }

  public render(): void {
    this.renderer.render(this.scene, this.camera);
    this.animate();
  }

  public addPolygonFromPoints(
    points: THREE.Vector3[],
    color: THREE.Color | string | number,
    id: string,
    closed: boolean = false
  ): void {
    if (points.length === 0) {
      return;
    }

    if (!closed) {
      points.push(points[0]);
    }

    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    this.addPolygonFromGeometry(geometry, color, id);
  }

  public addPolygonFromGeometry(
    geometry: THREE.BufferGeometry,
    color: THREE.Color | string | number,
    id: string
  ): void {
    const material = new THREE.LineBasicMaterial({ color });

    const line = new THREE.Line(geometry, material);
    line.uuid = id;
    this.scene.add(line);
  }

  public loadPolygon(file: File): void {
    const reader = new FileReader();

    reader.onload = () => {
      if (!reader.result) {
        return;
      }
      const obj = this.fbxLoader.parse(reader.result, '');
      obj.children
        .filter((x) => x instanceof THREE.Mesh)
        .forEach((x) => {
          const points = GraphicsUtils.numberArrayToVectors(
            Array.from((x as THREE.Mesh).geometry.attributes.position.array)
          );
          this.addPolygonFromPoints(
            points,
            0xff0000,
            Guid.create().toString(),
            true
          );
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

  private getCanvasPosition(event: MouseEvent): { x: number; y: number } {
    const rect = this.canvas.getBoundingClientRect();
    return {
      x: ((event.clientX - rect.left) / rect.width) * 2 - 1,
      y: ((event.clientY - rect.top) / rect.height) * -2 + 1,
    };
  }

  private pick(mousePos: { x: number; y: number }): void {
    this.raycaster.setFromCamera(mousePos, this.camera);
    const intersectedObjects = this.raycaster.intersectObjects(
      this.scene.children.filter(
        (x) => x instanceof THREE.Line && !(x instanceof THREE.AxesHelper)
      )
    );
    if (intersectedObjects.length === 0) {
      return;
    }
    this.pickedObject = intersectedObjects[0].object as THREE.Line;
  }

  private animate(): void {
    this.resizeCanvasToDisplaySize();
    this.renderer.render(this.scene, this.camera);
    requestAnimationFrame(this.animate.bind(this));
  }

  private updateGeometry(obj: THREE.Line): void {
    obj?.updateMatrix();
    obj?.geometry.applyMatrix4(obj.matrix);
    obj.matrix.identity();
    this.setToDefaults([obj]);
  }

  private onWheel(event: WheelEvent): void {
    event.preventDefault();

    this.camera.position.z += event.deltaY / 20;
  }

  private onMouseDown(event: MouseEvent): void {
    event.preventDefault();

    this.dragging = true;

    const pickingPos = this.getCanvasPosition(event);
    this.pick(pickingPos);
  }

  private onMouseUp(event: MouseEvent): void {
    event.preventDefault();

    this.dragging = false;

    if (this.pickedObject) {
      this.updateGeometry(this.pickedObject);
    }
    this.pickedObject = null;
  }

  private onMouseMove(event: MouseEvent): void {
    event.preventDefault();

    if (!this.dragging) {
      return;
    }

    if (this.pickedObject) {
      const pos = this.getCanvasPosition(event);
      this.raycaster.setFromCamera(pos, this.camera);
      const intersect = this.raycaster.intersectObject(this.workspacePlane)[0];
      this.pickedObject.position.copy(intersect.point);
      return;
    }

    this.camera.position.y += event.movementY * this.sensitivity;
    this.camera.position.x -= event.movementX * this.sensitivity;
  }

  private onKeyDown(event: KeyboardEvent): void {
    switch (event.key) {
      case 'Delete': {
        if (this.pickedObject) {
          this.scene.remove(this.pickedObject);
          this.render();
        }
        break;
      }
    }
  }
}
