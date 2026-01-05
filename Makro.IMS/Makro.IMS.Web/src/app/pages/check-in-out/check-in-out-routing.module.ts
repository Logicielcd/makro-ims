import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'guard-check-in',
    loadChildren: () =>
      import('./guard-check-in/guard-check-in.module').then((m) => m.GuardCheckInModule),    
  },
  {
    path: 'document-check-in',
    loadChildren: () =>
      import('./document-check-in/document-check-in.module').then((m) => m.DocumentCheckInModule),    
  },
  {
    path: 'guard-check-out',
    loadChildren: () =>
      import('./guard-check-out/guard-check-out.module').then((m) => m.GuardCheckOutModule),    
  },
  {
    path: 'guard-create-booking',
    loadChildren: () =>
      import('./guard-create-booking/guard-create-booking.module').then((m) => m.GuardCreateBookingModule),    
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CheckInOutRouterModule {}
