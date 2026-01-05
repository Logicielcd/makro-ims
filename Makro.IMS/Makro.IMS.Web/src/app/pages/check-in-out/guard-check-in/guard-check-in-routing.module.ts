import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GuardCheckInComponent } from './guard-check-in.component';

const routes: Routes = [
  { path: '', component: GuardCheckInComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GuardCheckInRoutingModule {}
