import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { Job } from '@core/models/transfer-management/job.model';
import { LanguageService } from '@core/services/language.service';
import { JobTransferService } from '@core/services/yard-management/job-transfer.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
interface CountStatus {
  total: number;
  new: number;
  assigned: number;
  inprocess: number;
  ondock: number;
  loading: number;
  loaded: number;
  completed: number;
  cancelled: number;
}

@UntilDestroy()
@Component({
  templateUrl: './Job-transfer-list.component.html',
})
export class JobTransferListComponent implements OnInit {
  jobs: Job[] = [];
  count_status: CountStatus = {
    total: 0,
    new: 0,
    assigned: 0,
    inprocess: 0,
    ondock: 0,
    loading: 0,
    loaded: 0,
    completed: 0,
    cancelled: 0,
  };

  statusDialogVisible: boolean = false;
  msgs: Message[] = [];
  isLoading: boolean = false;
  canModify = false;

  @ViewChild('f') f: NgForm;
  @ViewChild('dt') dt: Table;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    Validation: 'Message.Validation',
    ConfirmAssignTitle: 'Message.Confirm.ConfirmAssign.Title',
    ConfirmAssignMessage: 'Message.Confirm.ConfirmAssign.Message',
    ConfirmAssignSuccess: 'Message.Notification.ConfirmAssign',
    ConfirmStartTitle: 'Message.Confirm.ConfirmStart.Title',
    ConfirmStartMessage: 'Message.Confirm.ConfirmStart.Message',
    ConfirmStartSuccess: 'Message.Notification.ConfirmStart',
    ConfirmCompleteTitle: 'Message.Confirm.ConfirmComplete.Title',
    ConfirmCompleteMessage: 'Message.Confirm.ConfirmComplete.Message',
    ConfirmCompleteSuccess: 'Message.Notification.ConfirmComplete',
    CancelTitle: 'Message.Confirm.Cancel.Title',
    CancelMessage: 'Message.Confirm.Cancel.Message',
    CancelSuccess: 'Message.Notification.Cancel',

    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };
  constructor(
    private jobTransferService: JobTransferService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private translateService: TranslateService,
    private languageService: LanguageService,
    private authService: AuthService
  ) {
    this.authService
      .canModify(APP_MENU.transfer.module, APP_MENU.transfer.transferList)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));

    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
  }

  ngOnInit() {
    this.fetchData();
  }

  fetchData() {
    this.msgs = [];
    this.isLoading = true;
    this.jobTransferService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data_from_job) => {
        this.jobs = data_from_job;
        this.summary(data_from_job);
        this.isLoading = false;
      });
  }

  summary(data: Job[]) {
    this.count_status = {
      total: data.length,
      new: data.filter((data) => data.status === 'New').length,
      assigned: data.filter((data) => data.status === 'Assigned').length,
      inprocess: data.filter((data) => data.status === 'In-Process').length,
      ondock: data.filter((data) => data.status === 'On-Dock').length,
      loading: data.filter((data) => data.status === 'Loading').length,
      loaded: data.filter((data) => data.status === 'Loaded').length,
      completed: data.filter((data) => data.status === 'Completed').length,
      cancelled: data.filter((data) => data.status === 'Cancelled').length,
    };
  }

  showStatusDialog() {
    this.statusDialogVisible = true;
  }

  onConfirmAssignJob(job: Job) {
    this.msgs = [];
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.ConfirmAssignMessage
      ),
      header: this.translateService.instant(
        this.translatePrefix.ConfirmAssignTitle
      ),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.confirmAssignJob(job);
      },
    });
  }

  confirmAssignJob(job: Job) {
    this.jobTransferService
      .confirm_assign_job(job)
      .pipe(untilDestroyed(this))
      .subscribe(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Successful',
          detail: this.translateService.instant(
            `${this.translatePrefix.ConfirmAssignSuccess}`
          ),
          life: 3000,
        });
        this.refresh();
      });
  }

  onConfirmStartJob(job: Job) {
    this.msgs = [];
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.ConfirmStartMessage
      ),
      header: this.translateService.instant(
        this.translatePrefix.ConfirmStartTitle
      ),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.confirmStartJob(job);
      },
    });
  }

  confirmStartJob(job: Job) {
    this.jobTransferService
      .confirm_start_job(job)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.ConfirmStartSuccess}`
            ),
            life: 3000,
          });
          this.refresh();
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

  onConfirmCompleteJob(job: Job) {
    this.msgs = [];
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.ConfirmCompleteMessage
      ),
      header: this.translateService.instant(
        this.translatePrefix.ConfirmCompleteTitle
      ),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.confirmCompleteJob(job);
      },
    });
  }

  confirmCompleteJob(job: Job) {
    this.jobTransferService
      .confirm_complete_job(job)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.ConfirmCompleteSuccess}`
            ),
            life: 3000,
          });
          this.refresh();
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

  onCancelJob(job: Job) {
    this.msgs = [];
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.CancelMessage
      ),
      header: this.translateService.instant(this.translatePrefix.CancelTitle),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.cancelJob(job);
      },
    });
  }

  cancelJob(job: Job) {
    this.jobTransferService
      .cancel_job(job)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.CancelSuccess}`
            ),
            life: 3000,
          });
          this.refresh();
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

  refresh() {
    this.fetchData();
  }

  clear(table: Table, filterInput: HTMLInputElement) {
    filterInput.value = '';
    table.clear();
    table.sortOrder = 0;
    table.sortField = '';
    table.filterGlobal('', 'contains');
  }

  getSeverity(status: string): string {
    switch (status) {
      case 'New':
        return '#f15a22';
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
        return '#000000';
      case 'Cancelled':
        return '#6c757d';
      default:
        return '#6c757d';
    }
  }
}
