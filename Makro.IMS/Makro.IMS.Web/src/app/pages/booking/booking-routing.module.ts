import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'booking-header',
    loadChildren: () =>
      import('./booking-header/booking-header.module').then((m) => m.BookingHeaderModule),    
  },
  {
    path: 'create-booking',
    loadChildren: () =>
      import('./create-booking/create-booking.module').then((m) => m.CreateBookingModule),    
  },  
  {
    path: 'create-booking-excel',
    loadChildren: () =>
      import('./create-booking-excel/create-booking-excel.module').then((m) => m.CreateBookingExcelModule),    
  },  
  {
    path: 'create-booking-nopo',
    loadChildren: () =>
      import('./create-booking-nopo/create-booking-nopo.module').then((m) => m.CreateBookingNopoModule),    
  },  
  {
    path: 'check-po',
    loadChildren: () =>
      import('./check-po/check-po.module').then((m)=>m.CheckPoModule),
  },
  {
    path: 'po-monitor',
    loadChildren: () =>
      import('./po-monitor/po-monitor.module').then((m)=>m.PoMonitorModule),
  },
  {
    path: 'cap-monitor',
    loadChildren: () =>
      import('./cap-monitor/cap-monitor.module').then((m)=>m.CapMonitorModule),
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BookingRoutingModule {}
