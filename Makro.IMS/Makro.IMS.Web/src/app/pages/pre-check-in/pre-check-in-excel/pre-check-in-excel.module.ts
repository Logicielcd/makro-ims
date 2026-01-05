import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { PreCheckInExcelDetailComponent } from './detail/pre-check-in-excel-detail.component';
import { PreCheckInExcelRoutingModule } from './pre-check-in-excel-routing.module';
import { PreCheckInExcelListComponent } from './list/pre-check-in-excel-list.component';

import { ScheduleAllModule, RecurrenceEditorAllModule } from '@syncfusion/ej2-angular-schedule';
import { PoDialogComponent } from './detail/po-dialog/po-dialog.component';
import { UploadDialogComponent } from './list/upload-dialog/upload-dialog.component';

@NgModule({
  imports: [
    ThemeModule,
    PreCheckInExcelRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
    ScheduleAllModule,
    RecurrenceEditorAllModule,
  ],
  declarations: [
    PreCheckInExcelDetailComponent,
    PreCheckInExcelListComponent,        
    PoDialogComponent,
    UploadDialogComponent,
  ],
})
export class PreCheckInExcelModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/pre-check-in/pre-check-in-excel/',
    '.json'
  );
}
