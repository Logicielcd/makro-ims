import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { SupplierGroupDetailComponent } from './detail/supplier-group-detail.component';
import { SupplierGroupRoutingModule } from './supplier-group-routing.module';
import { SupplierGroupListComponent } from './list/supplier-group-list.component';

@NgModule({
  imports: [
    ThemeModule,
    SupplierGroupRoutingModule,
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
    SupplierGroupDetailComponent,
    SupplierGroupListComponent,    
  ],
})
export class SupplierGroupModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/master/supplier-group/',
    '.json'
  );
}
