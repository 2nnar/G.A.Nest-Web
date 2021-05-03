import { NgModule } from '@angular/core';
import { UiInfobarBottomComponent } from './ui-infobar-bottom/ui-infobar-bottom.component';
import { UiInfobarTopComponent } from './ui-infobar-top/ui-infobar-top.component';
import { UiSidebarLeftComponent } from './ui-sidebar-left/ui-sidebar-left.component';
import { UiSidebarRightComponent } from './ui-sidebar-right/ui-sidebar-right.component';
import { UiComponent } from './ui.component';

@NgModule({
  declarations: [
    UiComponent,
    UiInfobarBottomComponent,
    UiInfobarTopComponent,
    UiSidebarLeftComponent,
    UiSidebarRightComponent
  ],
  exports: [
    UiComponent
  ]
})
export class UiModule {}
