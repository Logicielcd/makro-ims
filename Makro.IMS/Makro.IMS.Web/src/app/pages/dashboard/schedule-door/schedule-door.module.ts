import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ScheduleDoorRoutingModule } from './schedule-door-routing.module';
import { ScheduleDoorComponent } from './schedule-door.component';
import { ScheduleAllModule, RecurrenceEditorAllModule } from '@syncfusion/ej2-angular-schedule';
import { RadioButtonModule } from '@syncfusion/ej2-angular-buttons';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    ScheduleDoorRoutingModule,
    ScheduleAllModule,
    RadioButtonModule,
    RecurrenceEditorAllModule,
    ThemeModule
  ],
  declarations: [ScheduleDoorComponent,],
})

export class ScheduleDoorModule {}
