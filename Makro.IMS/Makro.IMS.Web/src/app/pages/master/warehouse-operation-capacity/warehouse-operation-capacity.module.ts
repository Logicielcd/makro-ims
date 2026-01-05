import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { WarehouseOperationCapacityDetailComponent } from './detail/warehouse-operation-capacity-detail.component';
import { WarehouseOperationCapacityRoutingModule } from './warehouse-operation-capacity-routing.module';
import { WarehouseOperationCapacityListComponent } from './list/warehouse-operation-capacity-list.component';

@NgModule({
  imports: [
    ThemeModule,
    WarehouseOperationCapacityRoutingModule,
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
    WarehouseOperationCapacityDetailComponent,
    WarehouseOperationCapacityListComponent,    
  ],
})
export class WarehouseOperationCapacityModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/master/warehouse-operation-capacity/',
    '.json'
  );
}
