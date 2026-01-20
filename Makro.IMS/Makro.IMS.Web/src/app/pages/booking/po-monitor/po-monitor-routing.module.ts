import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PoMonitorComponent } from './po-monitor.component';

const routes: Routes = [{ path: '', component: PoMonitorComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PoMonitorRoutingModule {}
