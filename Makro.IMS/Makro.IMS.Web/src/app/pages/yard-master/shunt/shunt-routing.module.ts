import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ShuntDetailComponent } from './detail/shunt-detail.component';
import { ShuntListComponent } from './list/shunt-list.component';

const routes: Routes = [
  { path: '', component: ShuntListComponent },
  { path: 'create', component: ShuntDetailComponent },
  { path: ':id', component: ShuntDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ShuntRoutingModule {}
