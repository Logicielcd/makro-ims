import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupplierGroupDetailComponent } from './detail/supplier-group-detail.component';
import { SupplierGroupListComponent } from './list/supplier-group-list.component';

const routes: Routes = [
  { path: '', component: SupplierGroupListComponent },
  { path: 'create', component: SupplierGroupDetailComponent },
  { path: ':id', component: SupplierGroupDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SupplierGroupRoutingModule {}
