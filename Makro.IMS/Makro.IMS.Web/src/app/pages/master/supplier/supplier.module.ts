import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { SupplierDetailComponent } from './detail/supplier-detail.component';
import { SupplierRoutingModule } from './supplier-routing.module';
import { SupplierListComponent } from './list/supplier-list.component';

@NgModule({
  imports: [
    ThemeModule,
    SupplierRoutingModule,
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
    SupplierDetailComponent,
    SupplierListComponent,    
  ],
})
export class SupplierModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/master/supplier/',
    '.json'
  );
}
