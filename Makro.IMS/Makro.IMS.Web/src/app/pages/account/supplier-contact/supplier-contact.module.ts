import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { SupplierContactRoutingModule } from './supplier-contact-routing.module';
import { SupplierContactComponent } from './supplier-contact.component';

@NgModule({
  imports: [
    ThemeModule,
    SupplierContactRoutingModule,
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
    SupplierContactComponent,
  ],
})

export class SupplierContactModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/account/supplier-contact/',
    '.json'
  );
}
