import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
// import { WarehouseCapacityDetailComponent } from './detail/warehouse-capacity-detail.component';
import { GuardCreateBookingRoutingModule } from './guard-create-booking-routing.module';
import { GuardCreateBookingComponent } from './guard-create-booking.component';
// import { BookingHeaderListComponent } from '../booking-header/list/booking-header-list.component';


@NgModule({
  imports: [
    ThemeModule,
    GuardCreateBookingRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),    
  ],
  declarations: [
    GuardCreateBookingComponent,
    // BookingHeaderListComponent,    
  ],
})
export class GuardCreateBookingModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/booking/create-booking/',
    '.json'
  );
}
