import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Trailer } from '@core/models/master/trailer.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { TrailerService } from '@core/services/master/trailer.service';
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
  templateUrl: './trailer-list.component.html',
})
export class TrailerListComponent implements OnInit, OnDestroy {
  dataSource: Trailer[] = [];
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
    private trailerService: TrailerService,
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
        field: 'licensePlate',
        display: 'string',
        filter: 'string',
        header: 'Label.LicensePlate',
      },
      {
        field: 'locationId',
        display: 'string',
        filter: 'string',
        header: 'Label.Yard',
      },
      {
        field: 'trailerType',
        display: 'string',
        filter: 'string',
        header: 'Label.TrailerType',
      },
      {
        field: 'trailerGroup',
        display: 'string',
        filter: 'string',
        header: 'Label.TrailerGroup',
      },
      {
        field: 'trailerSize',
        display: 'string',
        filter: 'string',
        header: 'Label.TrailerSize',
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
        switchMap((sieve) => this.trailerService.getPaged(sieve)),
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
