import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Trailer } from '@core/models/master/trailer.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { JobOnDock } from '@core/models/transfer-management/job-on-dock.model';
import { LanguageService } from '@core/services/language.service';
import { JobOnDockService } from '@core/services/yard-management/job-on-dock.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { ChangeDoorModalComponent } from '../change-door-modal/change-door-modal.component';
import { ChangeTrailerModalComponent } from '../change-trailer-modal/change-trailer-modal.component';

@UntilDestroy()
@Component({
  templateUrl: './job-dock-door-list.component.html',
})
export class JobDockDoorListComponent implements OnInit {
  jobs: JobOnDock[] = [];
  msgs: Message[] = [];
  isLoading: boolean = false;
  canModify = false;
  ref: DynamicDialogRef;

  @ViewChild('f') f: NgForm;
  @ViewChild('dt') dt: Table;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    Validation: 'Message.Validation',
    ChangeDoorTitle: 'Message.Confirm.ChangeDoor.Title',
    ChangeDoorMessage: 'Message.Confirm.ChangeDoor.Message',
    ChangeDoorSuccess: 'Message.Notification.ChangeDoor',
    ConfirmLoadingTitle: 'Message.Confirm.ConfirmLoading.Title',
    ConfirmLoadingMessage: 'Message.Confirm.ConfirmLoading.Message',
    ConfirmLoadingSuccess: 'Message.Notification.ConfirmLoading',
    ConfirmLoadedTitle: 'Message.Confirm.ConfirmLoaded.Title',
    ConfirmLoadedMessage: 'Message.Confirm.ConfirmLoaded.Message',
    ConfirmLoadedSuccess: 'Message.Notification.ConfirmLoaded',
    ConfirmCompleteTitle: 'Message.Confirm.ConfirmComplete.Title',
    ConfirmCompleteMessage: 'Message.Confirm.ConfirmComplete.Message',
    ConfirmCompleteSuccess: 'Message.Notification.ConfirmComplete',

    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };
  constructor(
    private jobOnDockService: JobOnDockService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private translateService: TranslateService,
    private languageService: LanguageService,
    private authService: AuthService,
    private dialogService: DialogService
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
    this.jobOnDockService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data_from_job) => {
          this.jobs = data_from_job;
          this.isLoading = false;
        },
      });
  }

  // Loading confirmation methods
  onLoading(job: JobOnDock) {
    this.msgs = [];
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.ConfirmLoadingMessage
      ),
      header: this.translateService.instant(
        this.translatePrefix.ConfirmLoadingTitle
      ),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.loading(job);
      },
    });
  }

  loading(job: JobOnDock) {
    this.jobOnDockService
      .confirmLoading(job)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.ConfirmLoadingSuccess}`
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

  // Loaded confirmation methods
  onLoaded(job: JobOnDock) {
    this.msgs = [];
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.ConfirmLoadedMessage
      ),
      header: this.translateService.instant(
        this.translatePrefix.ConfirmLoadedTitle
      ),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.loaded(job);
      },
    });
  }

  loaded(job: JobOnDock) {
    this.jobOnDockService
      .confirmLoaded(job)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.ConfirmLoadedSuccess}`
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

  // Complete confirmation methods
  onComplete(job: JobOnDock) {
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
        this.complete(job);
      },
    });
  }

  complete(job: JobOnDock) {
    this.jobOnDockService
      .confirmComplete(job)
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

  onMoveTheDoor(job: JobOnDock) {
    this.msgs = [];
    this.ref = this.dialogService.open(ChangeDoorModalComponent, {
      header: 'Move Door',
      width: '30%',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: true,
      data: {
        job: job,
      },
    });

    this.ref.onClose.subscribe((result) => {
      this.refresh();
      if (result) {
        this.refresh();
      }
    });
  }

  onMoveTheTrailer(trailer: Trailer) {
    const ref = this.dialogService.open(ChangeTrailerModalComponent, {
      header: 'Assign Job',
      width: window.innerWidth <= 768 ? '100vw' : '35vw',
      data: { trailer: trailer },
      dismissableMask: true,
      modal: true,
      position: 'top',
    });

    ref.onClose.subscribe((result) => {
      this.fetchData();
      if (result) {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: `Job assigned to trailer ${trailer.licensePlate}`,
        });
      }
    });
  }

  refresh() {
    this.fetchData();
  }

  getYardSeverity(status: string): string {
    switch (status) {
      case 'Not Available':
        return 'danger';
      case 'Available':
        return 'success';
      default:
        return '';
    }
  }

  getJobSeverity(status: string): string {
    switch (status) {
      case 'New':
        return '#28a745';
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
      case 'Change Trailer':
        return '#198754';
      default:
        return '#ffffff';
    }
  }

  getTrailerSeverity(status: string): string {
    switch (status?.toLowerCase()) {
      case 'empty':
        return 'success';
      case 'full':
        return 'danger';
      case 'maintenance':
        return 'warning';
      case 'washing':
        return 'info';
      case 'ondock':
        return 'primary';
      case 'out-dc':
        return 'primary';
      default:
        return 'secondary';
    }
  }
}
