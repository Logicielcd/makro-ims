import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { CreateBookingExcelRoutingModule } from '../create-booking-excel/create-booking-excel-routing.module';
import { CreateBookingExcelComponent } from './create-booking-excel/create-booking-excel.component';


@NgModule({
  imports: [
    ThemeModule,
    CreateBookingExcelRoutingModule,
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
    CreateBookingExcelComponent,    
  ],
})
export class CreateBookingExcelModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/booking/create-booking-excel/',
    '.json'
  );
}
