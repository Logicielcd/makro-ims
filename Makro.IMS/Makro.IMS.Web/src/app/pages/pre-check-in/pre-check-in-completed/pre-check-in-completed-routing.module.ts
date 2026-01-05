import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PreCheckInCompletedDetailComponent } from './detail/pre-check-in-completed-detail.component';
import { PreCheckInCompletedListComponent } from './list/pre-check-in-completed-list.component';

const routes: Routes = [
  { path: '', component: PreCheckInCompletedListComponent },
  { path: ':internalHeaderKey', component: PreCheckInCompletedDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PreCheckInCompletedRoutingModule {}
