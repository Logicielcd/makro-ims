import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PagesComponent } from './pages.component';

const routes: Routes = [
  {
    path: '',
    component: PagesComponent,
    children: [
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
      },
      {
        path: 'master',
        loadChildren: () =>
          import('./master/master.module').then((m) => m.MasterModule),
      },
      {
        path: 'booking',
        loadChildren: () =>
          import('./booking/booking.module').then((m) => m.BookingModule),
      },
      {
        path: 'pre-check-in',
        loadChildren: () =>
          import('./pre-check-in/pre-check-in.module').then(
            (m) => m.PreCheckInModule
          ),
      },
      {
        path: 'check-in-out',
        loadChildren: () =>
          import('./check-in-out/check-in-out.module').then(
            (m) => m.CheckInOutModule
          ),
      },
      {
        path: 'queuemanage',
        loadChildren: () =>
          import('./queuemanage/queuemanage.module').then(
            (m) => m.QueueManageModule
          ),
      },
      {
        path: 'opsmanage',
        loadChildren: () =>
          import('./opsmanage/opsmanage.module').then((m) => m.OpsManageModule),
      },
      // {
      //   path: 'driver',
      //   loadChildren: () =>
      //     import('./driver/driver.module').then((m) => m.DriverModule),
      // },
      {
        path: 'report',
        loadChildren: () =>
          import('./report/report.module').then((m) => m.ReportModule),
      },
      {
        path: 'yard-master',
        loadChildren: () =>
          import('./yard-master/yard-master.module').then(
            (m) => m.YardMasterModule
          ),
      },
      {
        path: 'transfer',
        loadChildren: () =>
          import('./transfer-management/transfer-management.module').then(
            (m) => m.TransferManagementModule
          ),
      },
      {
        path: 'account',
        loadChildren: () =>
          import('./account/account.module').then((m) => m.AccountModule),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PagesRoutingModule {}
