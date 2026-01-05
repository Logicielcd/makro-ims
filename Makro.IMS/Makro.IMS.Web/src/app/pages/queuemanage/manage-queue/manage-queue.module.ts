import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { ManageQueueDetailComponent } from './detail/manage-queue-detail.component';
import { ManageQueueRoutingModule } from './manage-queue-routing.module';
import { ManageQueueListComponent } from './list/manage-queue-list.component';
import { CreateQueueDialogComponent } from './create-queue-dialog/create-queue-dialog.component';
import { SlotTimeDtlComponent } from './detail/dialog/slot-time-dtl.component';
import { SendDocumentDialogComponent } from './send-document-dialog/send-document-dialog.component';
import { DocumentCheckingDialogComponent } from './document-checking-dialog/document-checking-dialog.component';
import { ScheduleAllModule, RecurrenceEditorAllModule } from '@syncfusion/ej2-angular-schedule';
import { DocumentConfirmationDialogComponent } from './document-confirmation-dialog/document-confirmation-dialog.component';
import { UnloadFinishDialogComponent } from './unload-finish-dialog/unload-finish-dialog.component';
import { CancelPoDialogComponent } from './cancel-po-dialog/cancel-po-dialog.component';
import { DriverDialogComponent } from './driver-dialog/driver-dialog.component';

@NgModule({
  imports: [
    ThemeModule,
    ManageQueueRoutingModule,
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
    ManageQueueDetailComponent,
    ManageQueueListComponent,    
    SlotTimeDtlComponent,
    CreateQueueDialogComponent,
    SendDocumentDialogComponent,
    DocumentCheckingDialogComponent,
    DocumentConfirmationDialogComponent,
    UnloadFinishDialogComponent,
    CancelPoDialogComponent,
    DriverDialogComponent
  ],
})
export class ManageQueueModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/queuemanage/manage-queue/',
    '.json'
  );
}
