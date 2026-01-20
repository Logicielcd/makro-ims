import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GuardCheckOutComponent } from './guard-check-out.component';

const routes: Routes = [
  { path: '', component: GuardCheckOutComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GuardCheckOutRoutingModule {}
