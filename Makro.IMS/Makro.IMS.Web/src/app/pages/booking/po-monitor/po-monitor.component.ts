import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { User } from '@core/models/account/user.model';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { PoMonitor } from '@core/models/booking/po-monitor.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { PoService } from '@core/services/booking/po.service';
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
  templateUrl: 'po-monitor.component.html',
})
export class PoMonitorComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: PoMonitor[] = [];
  isCompleted: boolean = false;
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  lookup: any;

  export$: any[];
  ref: DynamicDialogRef;

  user: User;
  
  sieve: any;

  @ViewChild(TablePageListComponent) tableComp: TablePageListComponent;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    UploadSuccess: 'Message.Notification.Upload',
  };

  constructor(
    private poService: PoService,
    private warehouseService: WarehouseService,    
    private pagingService: PagingService,
    private translateService: TranslateService,
    private authService: AuthService
  ) 
  {
    this.authService
      .canModify(APP_MENU.booking.module, APP_MENU.booking.poMonitor)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
      this.user = this.authService.getUser();
  }

  ngOnInit() {
    this.cols = [      
      {
        field: 'warehouseCode',
        display: 'string',
        filter: 'string',
        header: 'Warehouse Code',
        //width: '20rem'
      },
      {
        field: 'companyCode',
        display: 'string',
        filter: 'string',
        header: 'Company Code',
        //width: '15rem'
      },
      {
        field: 'supCode',
        display: 'string',
        filter: 'string',
        header: 'Sup Code'
      },
      {
        field: 'supName',
        display: 'string',
        filter: 'string',
        header: 'Sup Name'
      },
      {
        field: 'bookingId',
        display: 'string',
        filter: 'string',
        header: 'BookingId',
      },          
      {
        field: 'bookingStart',
        display: 'datetime',
        filter: 'date',
        header: 'BookingStart',
      },
      {
        field: 'bookingEnd',
        display: 'datetime',
        filter: 'date',
        header: 'BookingEnd',
      },
      {
        field: 'bookingCreateDate',
        display: 'datetime',
        filter: 'date',
        header: 'Booking Created Date',
      },
      {
        field: 'userCreate',
        display: 'string',
        filter: 'string',
        header: 'User Create',
      },
      {
        field: 'poNbr',
        display: 'string',
        filter: 'string',
        header: 'Po No.',
      },      
      {
        field: 'poCreateDate',
        display: 'date',
        filter: 'date',
        header: 'PO Created Date',
      },
      {
        field: 'planReceivedDate',
        display: 'date',
        filter: 'date',
        header: 'Plan Received Date',
      },
      {
        field: 'expireDate',
        display: 'date',
        filter: 'date',
        header: 'PO Expire Date',
      },
      {
        field: 'remarkDelay',
        display: 'string',
        filter: 'string',
        header: 'Remark Delay'
      },
      {
        field: 'remarkBooking',
        display: 'string',
        filter: 'string',
        header: 'Remark Import'
      }      
    ];

    if(this.user.userType !== 'SUP' && this.user.userType !== 'SUPTRAN' && this.user.userType !== 'BH') {
      this.cols = this.commentCol(this.cols);
    }

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        switchMap((sieve) => 
          this.poService.getPoMonitor(sieve)
        ),
        untilDestroyed(this)
      )
      .subscribe({
        next: (paged) => {
          this.dataSource = paged.results;
          this.rowCount = paged.rowCount;
          this.pagingService.setLoading(false);          
          this.isCompleted = true;
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
          this.sieve = sieve;
        }),
        switchMap((sieve) => this.poService.getPoMonitorExport(sieve)),
        untilDestroyed(this)
      )
      .subscribe({
        next: (paged) => {
          this.export$ = paged.results;          
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

  commentCol(baseColumns: any[]) {
    const commentColumns = [
      { field: 'comment1', display: 'string', header: 'HREC' },
      { field: 'comment2', display: 'string', header: 'Do not hold' },
      { field: 'comment3', display: 'string', header: 'เลื่อนส่ง' },
      { field: 'comment4', display: 'string', header: 'ตัดยอด 1' },
      { field: 'comment5', display: 'string', header: "ตัดยอด 2"},
      { field: 'comment6', display: 'string', header: "อื่นๆ"}
    ];

    return [...baseColumns, ...commentColumns];
  }
}
