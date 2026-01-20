import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { DocumentConfirmationRoutingModule } from './document-confirmation-routing.module';
import { DocumentConfirmationComponent } from './document-confirmation.component';

@NgModule({
  imports: [
    ThemeModule,
    DocumentConfirmationRoutingModule,
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
    DocumentConfirmationComponent,
  ],
})
export class DocumentConfirmationModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/queuemanage/document-confirmation/',
    '.json'
  );
}
