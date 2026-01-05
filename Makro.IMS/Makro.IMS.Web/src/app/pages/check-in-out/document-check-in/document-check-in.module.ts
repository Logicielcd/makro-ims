import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { DocumentCheckInRoutingModule } from './document-check-in-routing.module';
import { DocumentCheckInComponent } from './document-check-in.component';

@NgModule({
  imports: [
    ThemeModule,
    DocumentCheckInRoutingModule,
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
    DocumentCheckInComponent,
  ],
})
export class DocumentCheckInModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/check-in-out/check-out/',
    '.json'
  );
}
