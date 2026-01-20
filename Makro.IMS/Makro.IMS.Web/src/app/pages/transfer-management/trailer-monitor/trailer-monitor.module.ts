import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { TrailerMonitorComponent } from './trailer-monitor.component';
import { TrailerMonitorRoutingModule } from './trailer-monitor-routing.module';

@NgModule({
  imports: [
    ThemeModule,
    TrailerMonitorRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  declarations: [TrailerMonitorComponent],
})
export class TrailerMonitorModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/transfer-management/trailer-monitor/',
    '.json'
  );
}
