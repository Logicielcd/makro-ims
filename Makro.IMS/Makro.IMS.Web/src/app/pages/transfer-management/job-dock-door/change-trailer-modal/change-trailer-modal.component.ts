import { JobOnDockService } from '@core/services/yard-management/job-on-dock.service';
import { Component, OnInit } from '@angular/core';
import { Door } from '@core/models/master/door.model';
import { Shunt } from '@core/models/master/shunt.model';
import { Yard } from '@core/models/master/yard.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { JobOnDock } from '@core/models/transfer-management/job-on-dock.model';
import { JobTransferRequest } from '@core/models/transfer-management/job.model';
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
import { ShuntSearchFullComponent } from '../../transfer-trailer-full/assign-job-full/shunt-search-full/shunt-search-full.component';
import { YardSearchFullComponent } from '../../transfer-trailer-full/assign-job-full/yard-search-full/yard-search-full.component';

interface JobTransferChangeTrailerRequest {
  jobId: string;
  trailerId: number;
  shuntId: number;
  locationId: number;
}
@UntilDestroy()
@Component({
  selector: 'app-change-trailer-modal',
  templateUrl: './change-trailer-modal.component.html',
  providers: [DialogService, MessageService, ConfirmationService],
})
export class ChangeTrailerModalComponent implements OnInit {
  trailer: JobOnDock;
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
    private JobOnDockService: JobOnDockService,
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
    const request: JobTransferChangeTrailerRequest = {
      jobId: this.trailer.jobId,
      trailerId: this.trailer.trailerId,
      shuntId: this.shunt.id,
      locationId: this.yard.id,
    };
    this.JobOnDockService.create_job_change_trailer(request)
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

  cancel() {
    this.ref.close();
  }
}
