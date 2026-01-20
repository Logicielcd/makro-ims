import { Component, OnInit, ViewChild } from '@angular/core';
import { Trailer } from '@core/models/master/trailer.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { TrailerService } from '@core/services/master/trailer.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { AssignJobEmptyComponent } from '../transfer-trailer-empty/assign-job-empty/assign-job-empty.component';
import { AssignJobFullComponent } from '../transfer-trailer-full/assign-job-full/assign-job-full.component';

interface CountStatus {
  total: number;
  empty: number;
  full: number;
  maintenance: number;
  washing: number;
  ondock: number;
  outdc: number;
}

@UntilDestroy()
@Component({
  selector: 'app-trailer-monitor',
  templateUrl: './trailer-monitor.component.html',
  styleUrls: ['./trailer-monitor.component.scss'],
})
export class TrailerMonitorComponent implements OnInit {
  trailers: Trailer[] = [];

  count_status: CountStatus = {
    total: 0,
    empty: 0,
    full: 0,
    maintenance: 0,
    washing: 0,
    ondock: 0,
    outdc: 0,
  };

  filters = {
    createDate: null,
    licensePlate: null,
    status: null,
    trailerGroup: null,
    trailerType: null,
    trailerSize: null,
    locationNo: null,
    locationType: null,
    jobStatus: null,
    jobDockStatus: null,
    locationZone: null,
  };

  statusOptions = [
    { label: 'Full', value: 'Full' },
    { label: 'Empty', value: 'Empty' },
    { label: 'On-Dock', value: 'On-Dock' },
    { label: 'Out-DC', value: 'Out-DC' },
  ];

  loading: boolean = false;
  canModify = false;
  @ViewChild('dtAll') dtAll!: Table;

  constructor(
    private trailerService: TrailerService,
    private authService: AuthService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {
    this.authService
      .canModify(APP_MENU.transfer.module, APP_MENU.transfer.trailerMonitor)
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
    this.clear(this.dtAll);
  }

  fetchData() {
    this.loading = true;
    this.count_status = null;
    this.trailerService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.trailers = data.sort((a, b) =>
            a.licensePlate.localeCompare(b.licensePlate)
          );
          this.summary(data);
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  summary(data: Trailer[]) {
    this.count_status = {
      total: data.length,
      empty: data.filter((trailer) => trailer.status === 'Empty').length,
      full: data.filter((trailer) => trailer.status === 'Full').length,
      maintenance: data.filter((trailer) =>
        trailer.status.includes('Maintenance')
      ).length,
      washing: data.filter((trailer) => trailer.status.includes('Washing'))
        .length,
      ondock: data.filter((trailer) => trailer.status.includes('On-Dock'))
        .length,
      outdc: data.filter((trailer) => trailer.status.includes('Out-DC')).length,
    };
  }

  check_status_before_open(trailer: Trailer) {
    if (trailer.status != 'Full' && trailer.status != 'On-Dock') {
      this.assign_job_empty(trailer);
    } else if (trailer.status === 'Full') {
      this.assign_job_full(trailer);
    }
  }

  assign_job_empty(trailer: Trailer) {
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

  assign_job_full(trailer: Trailer) {
    const ref = this.dialogService.open(AssignJobFullComponent, {
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

  onGlobalFilter(event: Event, tableType: 'all') {
    const searchValue = (event.target as HTMLInputElement).value;
    switch (tableType) {
      case 'all':
        this.dtAll.filterGlobal(searchValue, 'contains');
        break;
    }
  }

  getJobSeverity(status: string): string {
    switch (status) {
      case 'Loading':
        return '#e91e63';
      case 'Loaded':
        return '#9c27b0';
      case 'Change Trailer':
        return '#198754';
      default:
        return '#ffffff';
    }
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

  refresh() {
    this.fetchData();
  }

  clearFilters() {
    this.filters = {
      createDate: null,
      licensePlate: null,
      status: null,
      trailerGroup: null,
      trailerType: null,
      trailerSize: null,
      locationNo: null,
      locationType: null,
      jobStatus: null,
      jobDockStatus: null,
      locationZone: null,
    };
  }

  clear(table: Table) {
    table.sortOrder = 0;
    table.sortField = '';
    table.clear();
    this.fetchData();
    this.clearFilters();
  }
}
