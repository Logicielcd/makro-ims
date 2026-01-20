import { Component, OnInit } from '@angular/core';
import { Trailer } from '@core/models/master/trailer.model';
import { YardService } from '@core/services/master/yard.service';
import { JobTransferService } from '@core/services/yard-management/job-transfer.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

interface OutDCRequest {
  trailerId: number;
  outDcLicensePlate: string;
  outDcDriver: string;
}

@UntilDestroy()
@Component({
  selector: 'app-out-dc-dialog',
  templateUrl: './out-dc-dialog.component.html',
  providers: [ConfirmationService],
})
export class OutDcDialogComponent implements OnInit {
  trailer: Trailer;
  out_dc: OutDCRequest = {} as OutDCRequest;

  msgs: Message[] = [];
  isLoading: boolean = false;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    Validation: 'Message.Validation',
    OutDcSuccess: 'Message.Notification.OutDc',
    OutDcTitle: 'Message.Confirm.OutDc.Title',
    OutDcMessage: 'Message.Confirm.OutDc.Message',
    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };

  constructor(
    public jobTransferService: JobTransferService,
    public yardService: YardService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private translateService: TranslateService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit() {
    this.trailer = this.config.data.trailer;
    this.out_dc.trailerId = this.trailer.id;
    this.isLoading = true;
  }

  save() {
    this.confirmationService.confirm({
      message: this.translateService.instant(this.translatePrefix.OutDcMessage),
      header: this.translateService.instant(this.translatePrefix.OutDcTitle),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.isLoading = true;
        this.jobTransferService
          .update_out_dc(this.out_dc)
          .pipe(untilDestroyed(this))
          .subscribe({
            next: () => {
              this.messageService.add({
                severity: 'success',
                summary: 'Successful',
                detail: this.translateService.instant(
                  `${this.translatePrefix.OutDcSuccess}`
                ),
                life: 3000,
              });
              this.isLoading = false;
              this.cancel();
            },
            error: (error) => {
              this.msgs = [];
              error.Messages.forEach((msg: any) => {
                this.msgs.push({
                  severity: 'error',
                  summary: 'Error',
                  detail: this.translateService.instant(
                    `${this.translatePrefix.FromApi}.${msg}`
                  ),
                });
              });
              this.isLoading = false;
            },
          });
      },
    });
  }

  cancel() {
    this.ref.close();
  }
}
