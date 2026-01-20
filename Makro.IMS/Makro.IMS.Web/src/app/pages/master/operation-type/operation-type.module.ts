import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { OperationTypeDetailComponent } from './detail/operation-type-detail.component';
import { OperationTypeRoutingModule } from './operation-type-routing.module';
import { OperationTypeListComponent } from './list/operation-type-list.component';

@NgModule({
  imports: [
    ThemeModule,
    OperationTypeRoutingModule,
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
    OperationTypeDetailComponent,
    OperationTypeListComponent,    
  ],
})
export class OperationTypeModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/master/operation-type/',
    '.json'
  );
}
