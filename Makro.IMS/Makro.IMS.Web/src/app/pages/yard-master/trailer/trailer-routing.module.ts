import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TrailerDetailComponent } from './detail/trailer-detail.component';
import { TrailerListComponent } from './list/trailer-list.component';

const routes: Routes = [
  { path: '', component: TrailerListComponent },
  { path: 'create', component: TrailerDetailComponent },
  { path: ':id', component: TrailerDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TrailerRoutingModule {}
