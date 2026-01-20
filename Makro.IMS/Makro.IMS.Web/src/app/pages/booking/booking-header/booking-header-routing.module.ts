import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BookingHeaderDetailComponent } from './detail/booking-header-detail.component';
import { BookingHeaderListComponent } from './list/booking-header-list.component';

const routes: Routes = [
  { path: '', component: BookingHeaderListComponent },
  { path: ':internalHeaderKey', component: BookingHeaderDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BookingHeaderRoutingModule {}
