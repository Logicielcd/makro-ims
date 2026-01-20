import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { CreateBookingNopoRoutingModule } from './create-booking-nopo-routing.module';
import { CreateBookingNopoComponent } from './create-booking-nopo.component';


@NgModule({
  imports: [
    ThemeModule,
    CreateBookingNopoRoutingModule,
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
    CreateBookingNopoComponent,     
  ],
})
export class CreateBookingNopoModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/booking/create-booking/',
    '.json'
  );
}
