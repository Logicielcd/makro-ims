import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SummaryBookingComponent } from './summary-booking.component';

const routes: Routes = [{ path: '', component: SummaryBookingComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SummaryBookingRoutingModule {}
