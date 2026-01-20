import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReportRoutingModule } from './report-routing.module';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    ReportRoutingModule,
    ThemeModule
  ],
  declarations: [],
})
export class ReportModule {}
