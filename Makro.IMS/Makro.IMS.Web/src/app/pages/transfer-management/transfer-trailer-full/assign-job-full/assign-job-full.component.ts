import { Component, OnInit } from '@angular/core';
import { Shunt } from '@core/models/master/shunt.model';
import { Trailer } from '@core/models/master/trailer.model';
import { Yard } from '@core/models/master/yard.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { JobTransferService } from '@core/services/yard-management/job-transfer.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import {
  DialogService,
  DynamicDialogConfig,
  DynamicDialogRef,
} from 'primeng/dynamicdialog';
import { ShuntSearchFullComponent } from './shunt-search-full/shunt-search-full.component';
import { YardSearchFullComponent } from './yard-search-full/yard-search-full.component';
import { Door } from '@core/models/master/door.model';
import { JobTransferRequest } from '@core/models/transfer-management/job.model';
import { InDcDialogComponent } from '../../job-share-component/in-dc-dialog/in-dc-dialog.component';
import { OutDcDialogComponent } from '../../job-share-component/out-dc-dialog/out-dc-dialog.component';

@UntilDestroy()
@Component({
  selector: 'app-assign-job-full',
  templateUrl: './assign-job-full.component.html',
})
export class AssignJobFullComponent implements OnInit {
  trailer: Trailer;
  shunt: Shunt;
  yard: Yard;
  door: Door;

  msgs: Message[] = [];
  isLoading = false;
  canModify = false;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    Validation: 'Message.Validation',
    CreateSuccess: 'Message.Notification.Create',
    CreateTitle: 'Message.Confirm.Create.Title',
    CreateMessage: 'Message.Confirm.Create.Message',
    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };

  constructor(
    private authService: AuthService,
    private jobTransferService: JobTransferService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private dialogService: DialogService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private confirmationService: ConfirmationService
  ) {
    this.authService
      .canModify(
        APP_MENU.transfer.module,
        APP_MENU.transfer.transferTrailerFull
      )
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
    this.canModify = true;
  }

  ngOnInit() {
    this.trailer = this.config.data?.trailer;
  }

  openShuntSearch() {
    const ref = this.dialogService.open(ShuntSearchFullComponent, {
      header: 'Select Shunt',
      width: window.innerWidth <= 768 ? '100vw' : '70vw',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
    });

    ref.onClose.subscribe((selectedShunt: Shunt) => {
      if (selectedShunt) {
        this.shunt = selectedShunt;
        this.yard = null;
      }
    });
  }

  openYardSearch() {
    const ref = this.dialogService.open(YardSearchFullComponent, {
      header: 'Select Yard',
      width: window.innerWidth <= 768 ? '100vw' : '70vw',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
    });

    ref.onClose.subscribe((result: { yard: Yard; door: Door } | undefined) => {
      if (result) {
        this.yard = result.yard || null;
        this.door = result.door || null;
      }
    });
  }

  onCreateJob() {
    this.msgs = [];
    if (!this.shunt || !this.yard) {
      this.msgs = [
        {
          severity: 'error',
          summary: 'Error',
          detail: 'Please select both shunt and yard',
        },
      ];
      return;
    }
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.CreateMessage
      ),
      header: this.translateService.instant(this.translatePrefix.CreateTitle),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.createJob();
      },
    });
  }

  createJob() {
    this.isLoading = true;
    const request: JobTransferRequest = {
      trailerId: this.trailer.id,
      shuntId: this.shunt.id,
      locationId: this.yard.id,
      locationType: this.door?.doorName || null,
    };
    this.jobTransferService
      .create_job(request)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.CreateSuccess}`
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

  on_update_dc(status: string) {
    console.log(status);
    if (status === 'in_dc') {
      this.update_in_dc(this.trailer);
    } else if (status === 'out_dc') {
      this.update_out_dc(this.trailer);
    }
  }

  update_in_dc(trailer: Trailer) {
    const ref = this.dialogService.open(InDcDialogComponent, {
      header: 'Select Location In DC',
      width: window.innerWidth <= 768 ? '100vw' : '25vw',
      data: { trailer: trailer },
      dismissableMask: true,
      modal: true,
      position: 'center',
    });

    ref.onClose.subscribe((result) => {
      this.cancel();
    });
  }

  update_out_dc(trailer: Trailer) {
    const ref = this.dialogService.open(OutDcDialogComponent, {
      header: 'Trailer Dispatch',
      width: window.innerWidth <= 768 ? '100vw' : '25vw',
      data: { trailer: trailer },
      dismissableMask: true,
      modal: true,
      position: 'center',
    });

    ref.onClose.subscribe((result) => {
      this.cancel();
    });
  }

  getSeverity(status: string): string {
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

  cancel() {
    this.ref.close();
  }
}
