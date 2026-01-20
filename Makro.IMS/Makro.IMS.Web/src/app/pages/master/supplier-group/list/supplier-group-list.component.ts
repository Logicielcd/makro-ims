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
import { Supplier, SupplierGroup } from '@core/models/master/supplier.model';
import { SupplierGroupService } from '@core/services/master/supplier-group.service';

@UntilDestroy()
@Component({
  templateUrl: './supplier-group-list.component.html',
})
export class SupplierGroupListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: SupplierGroup[] = [];
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
    private pagingService: PagingService,
    private translateService: TranslateService,
    private authService: AuthService,
    private supplierGroupService: SupplierGroupService,
  ) {
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.supplierGroup)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  }

  ngOnInit() {
    this.cols = [
      {
        field: 'internalSupGroupId',
        display: 'string',
        filter: 'number',
        header: 'Label.Supplier',
        matchMode: 'equal',
      },
      {
        field: 'supName',
        display: 'string',
        filter: 'string',
        header: 'Label.SupName',
      },
      {
        field: 'contactName',
        display: 'string',
        filter: 'string',
        header: 'Label.ContactName',
      },
      {
        field: 'contactEMail',
        display: 'string',
        filter: 'string',
        header: 'Label.ContactEMail',
      },
      {
        field: 'phoneNumber',
        display: 'string',
        filter: 'string',
        header: 'Label.PhoneNumber',
      },
      {
        field: 'mobileNumber',
        display: 'string',
        filter: 'string',
        header: 'Label.MobileNumber',
      }
    ];

    this.pagingService.pagedData
    .pipe(
      tap(() => this.pagingService.setLoading(true)),
      switchMap((sieve) => 
        this.supplierGroupService.getPaged(sieve)
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
