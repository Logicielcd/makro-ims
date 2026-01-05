import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupplierDetailComponent } from './detail/supplier-detail.component';
import { SupplierListComponent } from './list/supplier-list.component';

const routes: Routes = [
  { path: '', component: SupplierListComponent },
  { path: 'create', component: SupplierDetailComponent },
  { path: ':id', component: SupplierDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SupplierRoutingModule {}
