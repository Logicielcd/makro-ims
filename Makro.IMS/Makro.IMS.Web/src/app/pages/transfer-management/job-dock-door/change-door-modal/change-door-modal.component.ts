import { Component, OnInit } from '@angular/core';
import { Door } from '@core/models/master/door.model';
import { Yard } from '@core/models/master/yard.model';
import { JobOnDock } from '@core/models/transfer-management/job-on-dock.model';
import { YardService } from '@core/services/master/yard.service';
import { JobOnDockService } from '@core/services/yard-management/job-on-dock.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
interface ChangeDoorRequest {
  jobId: number;
  locationId: number;
}

@UntilDestroy()
@Component({
  selector: 'app-change-door-modal',
  templateUrl: './change-door-modal.component.html',
  providers: [ConfirmationService, MessageService],
})
export class ChangeDoorModalComponent implements OnInit {
  job: JobOnDock;
  yards: Yard[] = [];
  selectedYard: Yard | null = null;
  isLoading: boolean = false;
  msgs: Message[] = [];

  translatePrefix = {
    FromApi: 'Message.FromApi',
    ChangeDoorTitle: 'Message.Confirm.ChangeDoor.Title',
    ChangeDoorMessage: 'Message.Confirm.ChangeDoor.Message',
    ChangeDoorSuccess: 'Message.Notification.ChangeDoor',
    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };

  constructor(
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private yardService: YardService,
    private jobOnDockService: JobOnDockService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private translateService: TranslateService
  ) {
    this.job = this.config.data.job;
  }

  ngOnInit() {
    this.loadYards();
  }

  loadYards() {
    this.isLoading = true;
    this.yardService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (yards) => {
          this.yards = yards.filter(
            (yard) => yard.status == 'Available' && yard.yardType == 'Dock'
          );

          console.log('Check Yards', this.yards);
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        },
      });
  }

  onSubmit() {
    if (this.selectedYard) {
      this.confirmationService.confirm({
        message: this.translateService.instant(
          this.translatePrefix.ChangeDoorMessage
        ),
        header: this.translateService.instant(
          this.translatePrefix.ChangeDoorTitle
        ),
        acceptLabel: this.translateService.instant(
          this.translatePrefix.YesButton
        ),
        rejectLabel: this.translateService.instant(
          this.translatePrefix.NoButton
        ),
        icon: 'pi pi-info-circle',
        accept: () => {
          this.changeDoor();
        },
      });
    }
  }

  changeDoor() {
    const changeDoorRequest: ChangeDoorRequest = {
      jobId: this.job.id,
      locationId: this.selectedYard.id,
    };

    this.isLoading = true;
    this.jobOnDockService
      .changeDoor(changeDoorRequest)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.ChangeDoorSuccess}`
            ),
            life: 3000,
          });
          this.ref.close(true);
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

  onCancel() {
    this.ref.close();
  }

  getSeverity(status: string): string {
    switch (status) {
      case 'New':
        return '#0d6efd';
      case 'Assigned':
        return '#0dcaf0';
      case 'In-Process':
        return '#ffc107';
      case 'On-Dock':
        return '#ff9800';
      case 'Loading':
        return '#e91e63';
      case 'Loaded':
        return '#9c27b0';
      case 'Completed':
        return '#198754';
      case 'Cancelled':
        return '#dc3545';
      default:
        return '#6c757d';
    }
  }
}
