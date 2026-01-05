import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageQueueDetailComponent } from './detail/manage-queue-detail.component';
import { ManageQueueListComponent } from './list/manage-queue-list.component';

const routes: Routes = [
  { path: '', component: ManageQueueListComponent },
  { path: ':internalHeaderKey', component: ManageQueueDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ManageQueueRoutingModule {}
