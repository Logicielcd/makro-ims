import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Trailer } from '@core/models/master/trailer.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { TrailerService } from '@core/services/master/trailer.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { Message, MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { AssignJobEmptyComponent } from './assign-job-empty/assign-job-empty.component';

@UntilDestroy()
@Component({
  templateUrl: './transfer-trailer-empty.component.html',
  providers: [MessageService],
})
export class TransferTrailerEmptyComponent implements OnInit {
  msgs: Message[] = [];
  isLoading: boolean = false;
  canModify = false;
  trailers: Trailer[] = [];

  @ViewChild('f') f: NgForm;
  @ViewChild('dt') dt: Table;

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };
  constructor(
    private trailerService: TrailerService,
    private authService: AuthService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService
  ) {
    this.authService
      .canModify(
        APP_MENU.transfer.module,
        APP_MENU.transfer.transferTrailerEmpty
      )
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
    this.trailerService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.trailers = data.filter((trailer) => trailer.status === 'Empty');
      });
  }

  assign_job_dialog(trailer: Trailer) {
    const ref = this.dialogService.open(AssignJobEmptyComponent, {
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

  clear(table: Table, inputElement: HTMLInputElement) {
    table.clear();
    inputElement.value = '';
    this.fetchData();
  }

  refresh() {
    this.fetchData();
  }

  getSeverity(status: string): string {
    switch (status) {
      case 'Empty':
        return 'success';
      case 'Full':
        return 'danger';
      case 'Maintenance':
        return 'warning';
      case 'Washing':
        return 'info';
      case 'On-Dock':
        return 'primary';
      case 'Out-DC':
        return 'primary';
      default:
        return '';
    }
  }
}
