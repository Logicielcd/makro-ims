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
import { SupplierService } from '@core/services/master/supplier.service';
import { Supplier } from '@core/models/master/supplier.model';

@UntilDestroy()
@Component({
  templateUrl: './supplier-list.component.html',
})
export class SupplierListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: Supplier[] = [];
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  export$: any[];
  warehouses: Warehouse[] = [];
  ref: DynamicDialogRef;

  sieve: any;

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
    private supplierService: SupplierService,
  ) {
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.supplier)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  }

  ngOnInit() {
    this.cols = [
      {
        field: 'supCode',
        display: 'string',
        filter: 'string',
        header: 'Label.Supplier',        
      },      
      {
        field: 'supName',
        display: 'string',
        filter: 'string',
        header: 'Label.SupName',
      },      
      {
        field: 'internalGroupId',
        display: 'string',
        filter: 'number',
        header: 'Label.SupGroupId',
      }
    ];

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        switchMap((sieve) =>           
          this.supplierService.getPaged(sieve)
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

      this.pagingService.exportData
      .pipe(
        tap((sieve) => {
          console.log('Export data sieve:', sieve); // Debugging line
          this.sieve = sieve;
        }),
        switchMap((sieve) => this.supplierService.getExport(sieve)),
        untilDestroyed(this)
      )
      .subscribe({
        next: (paged) => {
          this.export$ = paged.results;
          console.log('Export data paged:', paged); // Debugging line
          if (this.tableComp && typeof this.tableComp.exportExcel === 'function') {
            this.tableComp.exportExcel(paged.results, this.sieve.selected);
          } else {
            console.error('tableComp or exportExcel method is not defined');
          }
        },
        error: (err) => {
          console.error('Error exporting data:', err);
        }
      });
  }

  ngOnDestroy() {
    if (this.ref) {
      this.ref.close();
    }
  }
}
