import { Component, OnInit } from '@angular/core';
import { Trailer } from '@core/models/master/trailer.model';
import { Yard } from '@core/models/master/yard.model';
import { YardService } from '@core/services/master/yard.service';
import { JobTransferService } from '@core/services/yard-management/job-transfer.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { Message, MessageService } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

interface ChangeLocationRequest {
  trailerId: number;
  locationId: number;
}

@UntilDestroy()
@Component({
  selector: 'app-in-dc-dialog',
  templateUrl: './in-dc-dialog.component.html',
})
export class InDcDialogComponent implements OnInit {
  yards: Yard[] = [];
  trailer: Trailer;
  selectedYard: Yard | null = null;

  msgs: Message[] = [];
  isLoading: boolean = false;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    Validation: 'Message.Validation',
    InDcSuccess: 'Message.Notification.InDc',
    InDcTitle: 'Message.Confirm.InDc.Title',
    InDcMessage: 'Message.Confirm.InDc.Message',
    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };

  constructor(
    public jobTransferService: JobTransferService,
    public yardService: YardService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.trailer = this.config.data.trailer;
    this.isLoading = true;
    this.yardService.getAll().subscribe({
      next: (data) => {
        this.yards = data.filter(
          (yard) => yard.status === 'Available' && yard.yardType != 'Dock'
        );
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading yards:', error);
        this.isLoading = false;
      },
    });
  }

  save() {
    this.isLoading = true;
    const request: ChangeLocationRequest = {
      trailerId: this.trailer.id,
      locationId: this.selectedYard.id,
    };
    this.jobTransferService
      .update_in_dc(request)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.InDcSuccess}`
            ),
            life: 3000,
          });
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
  }

  cancel() {
    this.ref.close();
  }
}
