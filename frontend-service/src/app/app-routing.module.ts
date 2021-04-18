import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NestSceneComponent } from './components/nest-scene/nest-scene/nest-scene.component';

const routes: Routes = [{ path: '', component: NestSceneComponent }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
