import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateBookingNopoComponent } from './create-booking-nopo.component';
// import { BookingHeaderListComponent } from './list/booking-header-list.component';

const routes: Routes = [
  { path: '', component: CreateBookingNopoComponent },
  //  { path: 'create', component: WarehouseCapacityDetailComponent },
  // { path: ':internalHeaderId', component: BookingHeaderDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CreateBookingNopoRoutingModule {}
