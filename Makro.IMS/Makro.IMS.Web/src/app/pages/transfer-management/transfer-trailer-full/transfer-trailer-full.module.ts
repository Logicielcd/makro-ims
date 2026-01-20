import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { TransferTrailerFullRoutingModule } from './transfer-trailer-full-routing.module';
import { TransferTrailerFullComponent } from './transfer-trailer-full.component';
import { AssignJobFullComponent } from './assign-job-full/assign-job-full.component';
import { ShuntSearchFullComponent } from './assign-job-full/shunt-search-full/shunt-search-full.component';
import { YardSearchFullComponent } from './assign-job-full/yard-search-full/yard-search-full.component';
import { InDcDialogComponent } from '../job-share-component/in-dc-dialog/in-dc-dialog.component';
import { OutDcDialogComponent } from '../job-share-component/out-dc-dialog/out-dc-dialog.component';

@NgModule({
  imports: [
    ThemeModule,
    TransferTrailerFullRoutingModule,
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
    TransferTrailerFullComponent,
    AssignJobFullComponent,
    ShuntSearchFullComponent,
    YardSearchFullComponent,
    OutDcDialogComponent,
  ],
})
export class TransferTrailerFullModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/transfer-management/transfer-trailer-full/',
    '.json'
  );
}
