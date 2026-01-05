import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CapMonitorComponent } from './cap-monitor.component';

const routes: Routes = [{ path: '', component: CapMonitorComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CapMonitorRoutingModule {}
