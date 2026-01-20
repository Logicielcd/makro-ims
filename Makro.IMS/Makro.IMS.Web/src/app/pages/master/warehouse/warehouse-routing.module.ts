import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WarehouseDetailComponent } from './detail/warehouse-detail.component';
import { WarehouseListComponent } from './list/warehouse-list.component';

const routes: Routes = [
  { path: '', component: WarehouseListComponent },
  { path: 'create', component: WarehouseDetailComponent },
  { path: ':id', component: WarehouseDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class WarehouseRoutingModule {}
