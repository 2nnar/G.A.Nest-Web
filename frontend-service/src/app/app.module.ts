
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NestSceneModule } from './components/nest-scene/nest-scene.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NestSceneModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
