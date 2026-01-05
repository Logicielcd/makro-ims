import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EstimateTimeDetailComponent } from './detail/estimate-time-detail.component';
import { EstimateTimeListComponent } from './list/estimate-time-list.component';

const routes: Routes = [
  { path: '', component: EstimateTimeListComponent },
  { path: 'create', component: EstimateTimeDetailComponent },
  { path: ':id', component: EstimateTimeDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EstimateTimeRoutingModule {}
