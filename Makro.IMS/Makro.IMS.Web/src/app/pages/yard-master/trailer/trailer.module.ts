import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { TrailerDetailComponent } from './detail/trailer-detail.component';
import { TrailerListComponent } from './list/trailer-list.component';
import { TrailerRoutingModule } from './trailer-routing.module';
@NgModule({
  imports: [
    ThemeModule,
    TrailerRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  declarations: [TrailerListComponent, TrailerDetailComponent],
})
export class TrailerModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, 'assets/i18n/yard-master/trailer/', '.json');
}
