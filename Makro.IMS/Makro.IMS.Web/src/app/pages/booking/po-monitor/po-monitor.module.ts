import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { PoMonitorRoutingModule } from './po-monitor-routing.module';
import { PoMonitorComponent } from './po-monitor.component';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    PoMonitorRoutingModule,    
    ThemeModule
  ],
  declarations: [PoMonitorComponent,],
})

export class PoMonitorModule {}
