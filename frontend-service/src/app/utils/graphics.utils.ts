import * as THREE from 'three';
import { NestObjectPoint } from '../back-models/nest.model';

export class GraphicsUtils {
  public static numberArrayToVectors(array: number[]): THREE.Vector3[] {
    const vectors = [];
    for (let i = 0; i < array.length; i += 3) {
      const vector = new THREE.Vector3(array[i], array[i + 1], array[i + 2]);
      vectors.push(vector);
    }
    return vectors;
  }

  public static numberArrayToPoints(array: number[]): NestObjectPoint[] {
    const points = [];
    for (let i = 0; i < array.length; i += 3) {
      const point: NestObjectPoint = {
        x: array[i],
        y: array[i + 1],
        z: array[i + 2],
      };
      points.push(point);
    }
    return points;
  }
}
