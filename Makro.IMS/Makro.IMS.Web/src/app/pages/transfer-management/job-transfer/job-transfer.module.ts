import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { JobTransferRoutingModule } from './Job-transfer-routing.module';
import { JobTransferListComponent } from './list/Job-transfer-list.component';
@NgModule({
  imports: [
    ThemeModule,
    JobTransferRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  declarations: [JobTransferListComponent],
})
export class JobTransferModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/transfer-management/transfer-trailer/',
    '.json'
  );
}
