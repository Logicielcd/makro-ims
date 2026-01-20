import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'document-check-in',
    loadChildren: () =>
      import('./document-check-in/document-check-in.module').then((m) => m.DocumentCheckInModule),
  },
  {
    path: 'manage-queue',
    loadChildren: () =>
      import('./manage-queue/manage-queue.module').then((m) => m.ManageQueueModule),
  },
  // {
  //   path: 'dockdoor-list',
  //   loadChildren: () =>
  //     import('./dockdoor-list/dockdoor-list.module').then((m) => m.DockDoorListModule),
  // },  
  {
    path: 'document-confirmation',
    loadChildren: () =>
      import('./document-confirmation/document-confirmation.module').then((m) => m.DocumentConfirmationModule),
  },  
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class QueueManageRoutingModule {}
