import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GuardCreateBookingComponent } from './guard-create-booking.component';
// import { BookingHeaderListComponent } from './list/booking-header-list.component';

const routes: Routes = [
  { path: '', component: GuardCreateBookingComponent },
  //  { path: 'create', component: WarehouseCapacityDetailComponent },
  // { path: ':internalHeaderId', component: BookingHeaderDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GuardCreateBookingRoutingModule {}
