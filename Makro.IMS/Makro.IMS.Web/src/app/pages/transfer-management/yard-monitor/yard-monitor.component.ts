import { Component, OnInit, ViewChild } from '@angular/core';
import { YardMonitor } from '@core/models/master/yard.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { YardService } from '@core/services/master/yard.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { Table } from 'primeng/table';
interface CountStatus {
  total: number;
  available: number;
  notAvailable: number;
}
@UntilDestroy()
@Component({
  selector: 'app-yard-monitor',
  templateUrl: './yard-monitor.component.html',
})
export class YardMonitorComponent implements OnInit {
  yardAlls: YardMonitor[] = [];
  yardFulls: YardMonitor[] = [];
  yardEmptys: YardMonitor[] = [];
  
  count_status: CountStatus = {
    total: 0,
    available: 0,
    notAvailable: 0
  };

  loading: boolean = false;
  canModify = false;

  @ViewChild('dtAll') dtAll!: Table;
  @ViewChild('dtFull') dtFull!: Table;
  @ViewChild('dtEmpty') dtEmpty!: Table;

  constructor(
    private yardService: YardService,
    private authService: AuthService,
    private languageService: LanguageService,
    private translateService: TranslateService
  ) {
    this.authService
      .canModify(APP_MENU.transfer.module, APP_MENU.transfer.yardMonitor)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));

    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
  }

  ngOnInit() {
    this.fectchData();
  }

  fectchData() {
    this.loading = true;
    this.yardService
      .get_yard_monitor()
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.separate(data);
          this.summary(data);
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  separate(data: YardMonitor[]) {
    this.yardAlls = data.sort((a, b) => a.yardZone.localeCompare(b.yardZone));
    this.yardFulls = data
      .filter((yard) => yard.yardStatus === 'Not Available')
      .sort((a, b) => a.yardZone.localeCompare(b.yardZone));
    this.yardEmptys = data
      .filter((yard) => yard.yardStatus === 'Available')
      .sort((a, b) => a.yardZone.localeCompare(b.yardZone));
  }

  summary(data: YardMonitor[] | null) {
    if (data) {
      this.count_status = {
        total: data.length,
        available: data.filter((yard) => yard.yardStatus === 'Available')
          .length,
        notAvailable: data.filter((yard) => yard.yardStatus === 'Not Available')
          .length,
      };
    }
  }

  onGlobalFilter(event: Event, tableType: 'all' | 'full' | 'empty') {
    const searchValue = (event.target as HTMLInputElement).value;
    switch (tableType) {
      case 'all':
        this.dtAll.filterGlobal(searchValue, 'contains');
        break;
      case 'full':
        this.dtFull.filterGlobal(searchValue, 'contains');
        break;
      case 'empty':
        this.dtEmpty.filterGlobal(searchValue, 'contains');
        break;
    }
  }

  onRefresh() {
    this.fectchData();
  }

  clear(table: Table) {
    table.clear();
    table.sortOrder = 0;
    table.sortField = '';
  }

  getStatusYard(status: string): string {
    switch (status) {
      case 'Not Available':
        return 'danger';
      case 'Available':
        return 'success';
      default:
        return '';
    }
  }

  getStatusTrailer(status: string): string {
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
