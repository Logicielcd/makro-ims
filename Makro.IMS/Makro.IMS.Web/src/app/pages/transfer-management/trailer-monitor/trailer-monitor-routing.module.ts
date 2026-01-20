import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TrailerMonitorComponent } from './trailer-monitor.component';

const routes: Routes = [{ path: '', component: TrailerMonitorComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TrailerMonitorRoutingModule {}
