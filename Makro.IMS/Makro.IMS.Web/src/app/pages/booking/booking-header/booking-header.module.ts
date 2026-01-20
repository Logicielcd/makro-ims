import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { BookingHeaderDetailComponent } from './detail/booking-header-detail.component';
import { BookingHeaderRoutingModule } from './booking-header-routing.module';
import { BookingHeaderListComponent } from './list/booking-header-list.component';
import { SlotTimeDtlComponent } from './detail/dialog/slot-time-dtl.component';
import { PoDialogComponent } from './detail/po-dialog/po-dialog.component';
import { ScheduleAllModule, RecurrenceEditorAllModule } from '@syncfusion/ej2-angular-schedule';

@NgModule({
  imports: [
    ThemeModule,
    BookingHeaderRoutingModule,
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
    BookingHeaderDetailComponent,
    BookingHeaderListComponent,    
    SlotTimeDtlComponent,
    PoDialogComponent,
  ],
})
export class BookingHeaderModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/booking/booking-header/',
    '.json'
  );
}
