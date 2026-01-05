import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { DoorDetailComponent } from './detail/door-detail.component';
import { DoorRoutingModule } from './door-routing.module';
import { DoorListComponent } from './list/door-list.component';

@NgModule({
  imports: [
    ThemeModule,
    DoorRoutingModule,
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
    DoorDetailComponent,
    DoorListComponent,    
  ],
})
export class DoorModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/master/door/',
    '.json'
  );
}
