import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { PreCheckInCompletedDetailComponent } from './detail/pre-check-in-completed-detail.component';
import { PreCheckInCompletedRoutingModule } from './pre-check-in-completed-routing.module';
import { PreCheckInCompletedListComponent } from './list/pre-check-in-completed-list.component';

import { ScheduleAllModule, RecurrenceEditorAllModule } from '@syncfusion/ej2-angular-schedule';
import { PoDialogComponent } from './detail/po-dialog/po-dialog.component';
import { ReportDialogComponent } from './report/report-dialog.component';

@NgModule({
  imports: [
    ThemeModule,
    PreCheckInCompletedRoutingModule,
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
    PreCheckInCompletedDetailComponent,
    PreCheckInCompletedListComponent,        
    PoDialogComponent,
    ReportDialogComponent,
  ],
})
export class PreCheckInCompletedModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/pre-check-in/pre-check-in-completed/',
    '.json'
  );
}
