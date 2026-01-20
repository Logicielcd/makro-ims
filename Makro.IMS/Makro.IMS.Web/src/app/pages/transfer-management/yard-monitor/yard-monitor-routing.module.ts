import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { YardMonitorComponent } from './yard-monitor.component';

const routes: Routes = [{ path: '', component: YardMonitorComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class YardMonitorRoutingModule {}
