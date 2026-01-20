import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateBookingExcelComponent } from './create-booking-excel/create-booking-excel.component';
// import { BookingHeaderListComponent } from './list/booking-header-list.component';

const routes: Routes = [
  { path: '', component: CreateBookingExcelComponent },
  //  { path: 'create', component: WarehouseCapacityDetailComponent },
  // { path: ':internalHeaderId', component: BookingHeaderDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CreateBookingExcelRoutingModule {}
