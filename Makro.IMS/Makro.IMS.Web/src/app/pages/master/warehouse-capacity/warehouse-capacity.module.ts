import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { WarehouseCapacityDetailComponent } from './detail/warehouse-capacity-detail.component';
import { WarehouseCapacityRoutingModule } from './warehouse-capacity-routing.module';
import { WarehouseCapacityListComponent } from './list/warehouse-capacity-list.component';

@NgModule({
  imports: [
    ThemeModule,
    WarehouseCapacityRoutingModule,
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
    WarehouseCapacityDetailComponent,
    WarehouseCapacityListComponent,    
  ],
})
export class WarehouseCapacityModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/master/warehouse-capacity/',
    '.json'
  );
}
