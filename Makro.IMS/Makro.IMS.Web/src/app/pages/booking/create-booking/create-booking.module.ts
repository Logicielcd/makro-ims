import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
// import { WarehouseCapacityDetailComponent } from './detail/warehouse-capacity-detail.component';
import { CreateBookingRoutingModule } from '../create-booking/create-booking-routing.module';
import { CreateBookingComponent } from './create-booking/create-booking.component';
// import { BookingHeaderListComponent } from '../booking-header/list/booking-header-list.component';

import { ScheduleAllModule, RecurrenceEditorAllModule } from '@syncfusion/ej2-angular-schedule';


@NgModule({
  imports: [
    ThemeModule,
    CreateBookingRoutingModule,
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
    CreateBookingComponent,
    // BookingHeaderListComponent,    
  ],
})
export class CreateBookingModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/booking/create-booking/',
    '.json'
  );
}
