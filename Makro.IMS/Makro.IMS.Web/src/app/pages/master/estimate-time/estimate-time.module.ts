import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { EstimateTimeDetailComponent } from './detail/estimate-time-detail.component';
import { EstimateTimeRoutingModule } from './estimate-time-routing.module';
import { EstimateTimeListComponent } from './list/estimate-time-list.component';

@NgModule({
  imports: [
    ThemeModule,
    EstimateTimeRoutingModule,
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
    EstimateTimeDetailComponent,
    EstimateTimeListComponent,    
  ],
})
export class OperationTypeCapacityModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/master/estimate-time/',
    '.json'
  );
}
