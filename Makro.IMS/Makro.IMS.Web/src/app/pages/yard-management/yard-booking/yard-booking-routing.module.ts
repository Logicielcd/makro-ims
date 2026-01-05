import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { YardBookingDetailComponent } from './detail/yard-booking-detail.component';
import { YardBookingListComponent } from './list/yard-booking-list.component';

const routes: Routes = [
  { path: '', component: YardBookingListComponent },
  { path: ':internalHeaderKey', component: YardBookingDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class YardBookingRoutingModule {}
