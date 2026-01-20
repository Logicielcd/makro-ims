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

@UntilDestroy()
@Component({
  templateUrl: './door-list.component.html',
})
export class DoorListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: Door[] = [];
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
    private authService: AuthService
  ) {
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.door)
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
        field: 'doorName',
        display: 'string',
        filter: 'string',
        header: 'Label.DoorName',
      },
      {
        field: 'doorArea',
        display: 'string',
        filter: 'string',
        header: 'Label.DoorArea',
      },
      {
        field: 'truckType',
        display: 'string',
        filter: 'string',
        header: 'Label.TruckType',
      },
      {
        field: 'loadingType',
        display: 'string',
        filter: 'string',
        header: 'Label.LoadingType',
      },
      {
        field: 'sequence',
        display: 'string',
        filter: 'string',
        header: 'Label.Sequence',
      },
    ];

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        switchMap((sieve) => 
          this.doorService.getPaged(sieve)
        ),
        untilDestroyed(this)
      )
      .subscribe({
        next: (paged) => {
          this.dataSource = paged.results;
          
          this.dataSource.forEach(row => {
            row.id = row.internalDoorId;
          });

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
