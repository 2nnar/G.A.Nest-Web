export type NestObjectType = 'Unknown' | 'Group' | 'Polygon' | 'Curve';

export interface NestData {
  objects: NestObject[];
  bin: NestObject;
}

export interface NestObject {
  id: string;
  type: NestObjectType;
}

export interface NestCurve extends NestObject {
  center: NestObjectPoint;
  majorRadius: number;
  minorRadius: number;
  startParam: number;
  endParam: number;
}

export interface NestGroup extends NestObject {
  objects: NestObject[];
}

export interface NestPolygon extends NestObject {
  vertices: NestObjectPoint[];
}

export interface NestObjectPoint {
  x: number;
  y: number;
  z: number;
}

export interface NestResult {
  placements: NestObjectPlacement[];
}

export interface NestObjectPlacement {
  id: string;
  rotation: number;
  translationPoint: NestObjectPoint;
}

export interface NestConfig {
  tolerance: number;
  cutThickness: number;
  curveApproximation: number;
  rotationStep: number;
  iterationsCount: number;
  populationSize: number;
  mutationRate: number;
  holesUsing: boolean;
}

export interface GCodeData {
  objects: NestObject[];
}

export interface GCodeResult {
  commands: string[];
}
