import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleDoorComponent } from './schedule-door.component';

const routes: Routes = [{ path: '', component: ScheduleDoorComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ScheduleDoorRoutingModule {}
