import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { YardMonitorRoutingModule } from './yard-monitor-routing.module';
import { YardMonitorComponent } from './yard-monitor.component';

@NgModule({
  imports: [
    ThemeModule,
    YardMonitorRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  declarations: [YardMonitorComponent],
})
export class YardMonitorModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/transfer-management/yard-monitor/',
    '.json'
  );
}
