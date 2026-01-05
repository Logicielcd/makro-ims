import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Warehouse } from '@core/models/master/warehouse.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
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
  templateUrl: './warehouse-list.component.html',
})
export class WarehouseListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: Warehouse[] = [];
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
    private warehouseService: WarehouseService,    
    private pagingService: PagingService,
    private translateService: TranslateService,
    private authService: AuthService
  ) {
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.warehouse)
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
        field: 'warehouseName',
        display: 'string',
        filter: 'string',
        header: 'Label.WarehouseName',
      },
      {
        field: 'companyCode',
        display: 'string',
        filter: 'string',
        header: 'Label.CompanyCode',
      },      
      {
        field: 'warehouseMain',
        display: 'string',
        filter: 'string',
        header: 'Label.WarehouseMain',
      },     
      {
        field: 'warehouseLevel',
        display: 'string',
        filter: 'string',
        header: 'Label.WarehouseLevel',
      },
      {
        field: 'fixDoor',
        display: 'string',
        filter: 'string',
        header: 'Label.FixDoor',
      },
    ];

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        switchMap((sieve) => 
          this.warehouseService.getPaged(sieve)
        ),
        untilDestroyed(this)
      )
      .subscribe({
        next: (paged) => {
          this.dataSource = paged.results;
          
          this.dataSource.forEach(row => {
            row.id = row.warehouseCode;
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
