import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SummaryBookingRoutingModule } from './summary-booking-routing.module';
import { SummaryBookingComponent } from './summary-booking.component';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    SummaryBookingRoutingModule,
    ThemeModule
  ],
  declarations: [SummaryBookingComponent,],
})

export class SummaryBookingModule {}
