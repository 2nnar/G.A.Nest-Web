import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  NestConfig,
  NestData,
  NestResult,
} from 'src/app/back-models/nest.model';

@Injectable()
export class NestService {
  public constructor(private http: HttpClient) {}

  public async nest(data: NestData, config: NestConfig): Promise<NestResult> {
    let httpParams = new HttpParams();
    Object.keys(config).forEach((key) => {
      httpParams = httpParams.append(key, (config as any)[key]);
    });
    const result = await this.http
      .post<NestResult>('http://localhost:5005/api/v1/nest', data, {
        params: httpParams,
      })
      .toPromise();
    return result;
  }
}
