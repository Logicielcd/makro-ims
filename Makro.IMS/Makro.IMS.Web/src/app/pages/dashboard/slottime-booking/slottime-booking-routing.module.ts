import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SlottimeBookingComponent } from './slottime-booking.component';

const routes: Routes = [{ path: '', component: SlottimeBookingComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SlottimeBookingRoutingModule {}
