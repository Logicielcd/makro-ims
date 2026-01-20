import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { YardBookingDetailComponent } from './detail/yard-booking-detail.component';
import { YardBookingRoutingModule } from './yard-booking-routing.module';
import { YardBookingListComponent } from './list/yard-booking-list.component';
import { SlotTimeDtlComponent } from './detail/dialog/slot-time-dtl.component';
import { ScheduleAllModule, RecurrenceEditorAllModule } from '@syncfusion/ej2-angular-schedule';

@NgModule({
  imports: [
    ThemeModule,
    YardBookingRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
    ScheduleAllModule,
    RecurrenceEditorAllModule,
  ],
  declarations: [
    YardBookingDetailComponent,
    YardBookingListComponent,    
    SlotTimeDtlComponent,
  ],
})
export class YardBookingModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/booking/booking-header/',
    '.json'
  );
}
