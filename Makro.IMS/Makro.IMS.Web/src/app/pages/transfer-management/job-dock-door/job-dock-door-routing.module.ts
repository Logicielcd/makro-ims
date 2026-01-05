import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { JobDockDoorListComponent } from './list/job-dock-door-list.component';

const routes: Routes = [{ path: '', component: JobDockDoorListComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class JobDockDoorRoutingModule {}
