import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { WarehouseDetailComponent } from './detail/warehouse-detail.component';
import { WarehouseRoutingModule } from './warehouse-routing.module';
import { WarehouseListComponent } from './list/warehouse-list.component';

@NgModule({
  imports: [
    ThemeModule,
    WarehouseRoutingModule,
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
    WarehouseDetailComponent,
    WarehouseListComponent,    
  ],
})
export class WarehouseModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/master/warehouse/',
    '.json'
  );
}
