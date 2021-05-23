import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  GCodeData,
  GCodeResult,
  NestConfig,
  NestData,
  NestResult,
} from 'src/app/back-models/nest.model';

@Injectable()
export class NestService {
  private baseUrl = 'http://g-a-nest.ru/api';

  public constructor(private http: HttpClient) {}

  public async nest(data: NestData, config: NestConfig): Promise<NestResult> {
    let httpParams = new HttpParams();
    Object.keys(config).forEach((key) => {
      httpParams = httpParams.append(key, (config as any)[key]);
    });
    const result = await this.http
      .post<NestResult>(`${this.baseUrl}/v1/nest`, data, {
        params: httpParams,
      })
      .toPromise();
    return result;
  }

  public async getGCode(data: GCodeData): Promise<GCodeResult> {
    const result = await this.http
      .post<GCodeResult>(`${this.baseUrl}/v1/g-code`, data)
      .toPromise();
    return result;
  }
}
