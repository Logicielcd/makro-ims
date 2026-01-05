import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WarehouseOperationCapacityDetailComponent } from './detail/warehouse-operation-capacity-detail.component';
import { WarehouseOperationCapacityListComponent } from './list/warehouse-operation-capacity-list.component';

const routes: Routes = [
  { path: '', component: WarehouseOperationCapacityListComponent },
  { path: 'create', component: WarehouseOperationCapacityDetailComponent },
  { path: ':id', component: WarehouseOperationCapacityDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class WarehouseOperationCapacityRoutingModule {}
