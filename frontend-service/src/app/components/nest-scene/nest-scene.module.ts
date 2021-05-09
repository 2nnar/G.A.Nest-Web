import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { NestSceneComponent } from './nest-scene/nest-scene.component';
import { EngineService } from './services/engine.service';
import { NestService } from './services/nest.service';

@NgModule({
  declarations: [NestSceneComponent],
  exports: [NestSceneComponent],
  providers: [EngineService, NestService],
  imports: [NgxDropzoneModule, CommonModule, HttpClientModule],
})
export class NestSceneModule {}
