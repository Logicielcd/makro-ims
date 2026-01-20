import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { YardDetailComponent } from './detail/yard-detail.component';
import { YardListComponent } from './list/yard-list.component';

const routes: Routes = [
  { path: '', component: YardListComponent },
  { path: 'create', component: YardDetailComponent },
  { path: ':id', component: YardDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class YardRoutingModule {}

