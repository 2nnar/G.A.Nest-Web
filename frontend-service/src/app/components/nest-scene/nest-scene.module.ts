import { NgModule } from '@angular/core';
import { NestSceneComponent } from './nest-scene/nest-scene.component';
import { EngineService } from './services/engine.service';
import { UiModule } from '../ui/ui.module';

@NgModule({
  declarations: [NestSceneComponent],
  exports: [NestSceneComponent],
  providers: [EngineService],
  imports: [UiModule]
})
export class NestSceneModule {}
