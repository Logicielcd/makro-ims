import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OperationTypeDetailComponent } from './detail/operation-type-detail.component';
import { OperationTypeListComponent } from './list/operation-type-list.component';

const routes: Routes = [
  { path: '', component: OperationTypeListComponent },
  { path: 'create', component: OperationTypeDetailComponent },
  { path: ':id', component: OperationTypeDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OperationTypeRoutingModule {}
