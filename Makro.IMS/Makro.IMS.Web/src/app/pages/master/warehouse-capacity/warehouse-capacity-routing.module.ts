import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WarehouseCapacityDetailComponent } from './detail/warehouse-capacity-detail.component';
import { WarehouseCapacityListComponent } from './list/warehouse-capacity-list.component';

const routes: Routes = [
  { path: '', component: WarehouseCapacityListComponent },
  { path: 'create', component: WarehouseCapacityDetailComponent },
  { path: ':id', component: WarehouseCapacityDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class WarehouseCapacityRoutingModule {}
