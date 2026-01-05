import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { DashboardsRoutingModule } from './dashboard-routing.module';
import { ScheduleAllModule, RecurrenceEditorAllModule } from '@syncfusion/ej2-angular-schedule';
import { RadioButtonModule } from '@syncfusion/ej2-angular-buttons';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    DashboardsRoutingModule,
    ScheduleAllModule,
    RadioButtonModule,
    RecurrenceEditorAllModule,
    ThemeModule
  ],
  declarations: [],
})
export class DashboardModule {}
