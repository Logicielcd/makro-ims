import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { YardDetailComponent } from './detail/yard-detail.component';
import { YardRoutingModule } from './yard-routing.module';
import { YardListComponent } from './list/yard-list.component';

@NgModule({
  imports: [
    ThemeModule,
    YardRoutingModule,
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
    YardDetailComponent,
    YardListComponent,    
  ],
})
export class YardModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/yard-master/yard/',
    '.json'
  );
}
