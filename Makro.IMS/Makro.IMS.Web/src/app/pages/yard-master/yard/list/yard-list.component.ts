import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Yard } from '@core/models/master/yard.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { YardService } from '@core/services/master/yard.service';
import { PagingService } from '@core/services/paging.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { TablePageListComponent } from '@theme/components/table-page-list/table-page-list.component';
import { AuthService } from 'auth/auth.service';
import { Message } from 'primeng/api';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { switchMap, tap } from 'rxjs';

@UntilDestroy()
@Component({
  templateUrl: './yard-list.component.html',
})
export class YardListComponent implements OnInit, OnDestroy {
  dataSource: Yard[] = [];

  cols: any[];
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;
  ref: DynamicDialogRef;

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  @ViewChild(TablePageListComponent) tableComp: TablePageListComponent;

  constructor(
    private yardService: YardService,
    private pagingService: PagingService,
    private translateService: TranslateService,
    private authService: AuthService
  ) {
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.supplier)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  }

  ngOnInit() {
    this.cols = [
      {
        field: 'yardNo',
        display: 'string',
        filter: 'string',
        header: 'Label.YardNo',
      },
      {
        field: 'yardType',
        display: 'string',
        filter: 'string',
        header: 'Label.YardType',
      },
      {
        field: 'yardZone',
        display: 'string',
        filter: 'string',
        header: 'Label.YardZone',
      },
      {
        field: 'locationNo',
        display: 'string',
        filter: 'string',
        header: 'Label.LocationNo',
      },
      {
        field: 'trailerType',
        display: 'string',
        filter: 'string',
        header: 'Label.TrailerType',
      },
      {
        field: 'status',
        display: 'string',
        filter: 'string',
        header: 'Label.Status',
      },
      {
        field: 'userStamp',
        display: 'string',
        filter: 'string',
        header: 'Label.UserStamp',
      },
      {
        field: 'createDate',
        display: 'datetime',
        filter: 'date',
        header: 'Label.CreateDate',
      },
      {
        field: 'modDate',
        display: 'datetime',
        filter: 'date',
        header: 'Label.ModDate',
      },
    ];

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        switchMap((sieve) => this.yardService.getPaged(sieve)),
        untilDestroyed(this)
      )
      .subscribe({
        next: (paged) => {
          this.dataSource = paged.results;
          this.rowCount = paged.rowCount;
          this.pagingService.setLoading(false);
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
        },
      });
  }
  ngOnDestroy() {
    if (this.ref) {
      this.ref.close();
    }
  }
}
