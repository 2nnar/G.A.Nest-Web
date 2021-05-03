import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { NestSceneComponent } from './nest-scene/nest-scene.component';
import { EngineService } from './services/engine.service';

@NgModule({
  declarations: [NestSceneComponent],
  exports: [NestSceneComponent],
  providers: [EngineService],
  imports: [NgxDropzoneModule, CommonModule],
})
export class NestSceneModule {}
