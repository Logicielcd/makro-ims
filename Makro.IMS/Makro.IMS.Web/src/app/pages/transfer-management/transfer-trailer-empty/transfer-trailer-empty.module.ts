import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { InDcDialogComponent } from '../job-share-component/in-dc-dialog/in-dc-dialog.component';
import { OutDcDialogComponent } from '../job-share-component/out-dc-dialog/out-dc-dialog.component';
import { AssignJobEmptyComponent } from './assign-job-empty/assign-job-empty.component';
import { ShuntSearchEmptyComponent } from './assign-job-empty/shunt-search-empty/shunt-search-empty.component';
import { YardSearchEmptyComponent } from './assign-job-empty/yard-search-empty/yard-search-empty.component';
import { TransferTrailerEmptyRoutingModule } from './transfer-trailer-empty-routing.module';
import { TransferTrailerEmptyComponent } from './transfer-trailer-empty.component';

@NgModule({
  imports: [
    ThemeModule,
    TransferTrailerEmptyRoutingModule,
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
    TransferTrailerEmptyComponent,
    AssignJobEmptyComponent,
    ShuntSearchEmptyComponent,
    YardSearchEmptyComponent,
    InDcDialogComponent,
  ],
})
export class TransferTrailerEmptyModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/transfer-management/transfer-trailer-empty/',
    '.json'
  );
}
