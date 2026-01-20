import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TruckStatusRoutingModule } from './truck-status-routing.module';
import { TruckStatusComponent } from './truck-status.component';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    TruckStatusRoutingModule,
    ThemeModule
  ],
  declarations: [TruckStatusComponent,],
})

export class TruckStatusModule {}
