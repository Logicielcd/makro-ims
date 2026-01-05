import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Warehouse } from '@core/models/master/warehouse.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { Door } from '@core/models/master/door.model';
import { DoorService } from '@core/services/master/door.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { PagingService } from '@core/services/paging.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { TablePageListComponent } from '@theme/components/table-page-list/table-page-list.component';
import { AuthService } from 'auth/auth.service';
import { Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { switchMap, tap } from 'rxjs';
import { OperationService } from '@core/services/master/operation.service';
import { Operation } from '@core/models/master/operation.model';

@UntilDestroy()
@Component({
  templateUrl: './operation-type-list.component.html',
})
export class OperationTypeListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: Operation[] = [];
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  warehouses: Warehouse[] = [];
  ref: DynamicDialogRef;

  @ViewChild(TablePageListComponent) tableComp: TablePageListComponent;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    UploadSuccess: 'Message.Notification.Upload',
  };

  constructor(
    private warehouseService: WarehouseService,
    private doorService: DoorService,
    private pagingService: PagingService,
    private translateService: TranslateService,
    private authService: AuthService,
    private operationService: OperationService,
  ) {
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.operationType)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  }

  ngOnInit() {
    this.cols = [
      {
        field: 'warehouseCode',
        display: 'string',
        filter: 'string',
        header: 'Label.WarehouseCode',
        width: '150px',
      },      
      {
        field: 'operationName',
        display: 'string',
        filter: 'string',
        header: 'Label.OperationName',
      },
      {
        field: 'description',
        display: 'string',
        filter: 'string',
        header: 'Label.Description',
      },      
    ];

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        switchMap((sieve) => 
          this.operationService.getPaged(sieve)
        ),
        untilDestroyed(this)
      )
      .subscribe({
        next: (paged) => {
          paged.results.forEach(o => {
            o.searchkey = o.warehouseCode + '|' + o.operationName;
          });
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
