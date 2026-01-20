import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoorDetailComponent } from './detail/door-detail.component';
import { DoorListComponent } from './list/door-list.component';

const routes: Routes = [
  { path: '', component: DoorListComponent },
  { path: 'create', component: DoorDetailComponent },
  { path: ':id', component: DoorDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DoorRoutingModule {}
