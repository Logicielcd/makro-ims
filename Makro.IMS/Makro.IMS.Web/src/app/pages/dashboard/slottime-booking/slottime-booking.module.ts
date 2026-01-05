import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SlottimeBookingRoutingModule } from './slottime-booking-routing.module';
import { SlottimeBookingComponent } from './slottime-booking.component';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    SlottimeBookingRoutingModule,    
    ThemeModule
  ],
  declarations: [SlottimeBookingComponent,],
})

export class SlottimeBookingModule {}
