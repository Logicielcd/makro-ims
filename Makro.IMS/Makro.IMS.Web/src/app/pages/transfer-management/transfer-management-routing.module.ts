import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'yard-monitor',
    loadChildren: () =>
      import('./yard-monitor/yard-monitor.module').then(
        (m) => m.YardMonitorModule
      ),
  },
  {
    path: 'trailer-monitor',
    loadChildren: () =>
      import('./trailer-monitor/trailer-monitor.module').then(
        (m) => m.TrailerMonitorModule
      ),
  },
  {
    path: 'transfer-trailer-empty',
    loadChildren: () =>
      import('./transfer-trailer-empty/transfer-trailer-empty.module').then(
        (m) => m.TransferTrailerEmptyModule
      ),
  },
  {
    path: 'transfer-trailer-full',
    loadChildren: () =>
      import('./transfer-trailer-full/transfer-trailer-full.module').then(
        (m) => m.TransferTrailerFullModule
      ),
  },
  {
    path: 'job-transfer',
    loadChildren: () =>
      import('./job-transfer/job-transfer.module').then(
        (m) => m.JobTransferModule
      ),
  },
  {
    path: 'job-dock-door',
    loadChildren: () =>
      import('./job-dock-door/job-dock-door.module').then(
        (m) => m.JobDockDoorModule
      ),
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TransferManagementRoutingModule {}
