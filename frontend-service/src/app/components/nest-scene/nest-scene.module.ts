import { NgModule } from '@angular/core';
import { NestSceneComponent } from './nest-scene/nest-scene.component';
import { WebGLService } from './services/web-gl.service';

@NgModule({
  declarations: [NestSceneComponent],
  exports: [NestSceneComponent],
  providers: [WebGLService]
})
export class NestSceneModule {}
