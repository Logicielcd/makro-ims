import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TruckStatusComponent } from './truck-status.component';

const routes: Routes = [{ path: '', component: TruckStatusComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TruckStatusRoutingModule {}
