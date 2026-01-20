import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CapMonitorRoutingModule } from './cap-monitor-routing.module';
import { CapMonitorComponent } from './cap-monitor.component';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    CapMonitorRoutingModule,    
    ThemeModule
  ],
  declarations: [CapMonitorComponent,],
})

export class CapMonitorModule {}
