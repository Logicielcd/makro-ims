import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { WarehouseOperationCapacity } from '@core/models/master/warehouse-operation-capacity.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { WarehouseOperationCapacityService } from '@core/services/master/warehouse-operation-capacity.service';
import { PagingService } from '@core/services/paging.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { TablePageListComponent } from '@theme/components/table-page-list/table-page-list.component';
import { AuthService } from 'auth/auth.service';
import { Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { switchMap, tap } from 'rxjs';

@UntilDestroy()
@Component({
  templateUrl: './warehouse-operation-capacity-list.component.html',
})
export class WarehouseOperationCapacityListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: WarehouseOperationCapacity[] = [];
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  ref: DynamicDialogRef;

  @ViewChild(TablePageListComponent) tableComp: TablePageListComponent;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    UploadSuccess: 'Message.Notification.Upload',
  };

  constructor(
    private warehouseOperationCapacityService: WarehouseOperationCapacityService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private pagingService: PagingService,
    private translateService: TranslateService,
    private authService: AuthService
  ) {
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.warehouseOperationCapacity)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  }

  ngOnInit() {
  //  this.canModify = true;

    this.cols = [
      {
        field: 'warehouseCode',
        display: 'string',
        filter: 'string',
        header: 'Label.WarehouseCode',
      },
      {
        field: 'operationType',
        display: 'string',
        filter: 'string',
        header: 'Label.OperationType',
      },
      {
        field: 'bookingDate',
        display: 'string',
        filter: 'date',
        header: 'Label.BookingDate',
      },
      {
        field: 'maxConPerHour',
        display: 'string',
        filter: 'string',
        header: 'Label.MaxCon',
      },
      {
        field: 'maxNonPerHour',
        display: 'string',
        filter: 'string',
        header: 'Label.MaxNon',
      },
      {
        field: 'maxFullPerHour',
        display: 'string',
        filter: 'string',
        header: 'Label.MaxFull',
      },
      {
        field: 'maxAllPerHour',
        display: 'string',
        filter: 'string',
        header: 'Label.MaxAll',
      },
      {
        field: 'maxCConPerHour',
        display: 'string',
        filter: 'string',
        header: 'Label.MaxCCon',
      },
      {
        field: 'maxCNonPerHour',
        display: 'string',
        filter: 'string',
        header: 'Label.MaxCNon',
      },
      {
        field: 'maxCFullPerHour',
        display: 'string',
        filter: 'string',
        header: 'Label.MaxCFull',
      },
      {
        field: 'maxCAllPerHour',
        display: 'string',
        filter: 'string',
        header: 'Label.MaxCAll',
      },
    ];

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        switchMap((sieve) => 
          this.warehouseOperationCapacityService.getPaged(sieve)
        ),
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
