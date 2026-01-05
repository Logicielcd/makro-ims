import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { ShuntDetailComponent } from './detail/shunt-detail.component';
import { ShuntListComponent } from './list/shunt-list.component';
import { ShuntRoutingModule } from './shunt-routing.module';
@NgModule({
  imports: [
    ThemeModule,
    ShuntRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  declarations: [ShuntListComponent, ShuntDetailComponent],
})
export class ShuntModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/yard-master/shunt/',
    '.json'
  );
}
