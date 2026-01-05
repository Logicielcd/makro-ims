import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
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
  templateUrl: './yard-booking-list.component.html',
})
export class YardBookingListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: BookingHeader[] = [];
  isCompleted: boolean = false;
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  lookup: any;

  ref: DynamicDialogRef;

  @ViewChild(TablePageListComponent) tableComp: TablePageListComponent;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    UploadSuccess: 'Message.Notification.Upload',
  };

  constructor(
    private bookingHeaderService: BookingHeaderService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private pagingService: PagingService,
    private translateService: TranslateService,
    private authService: AuthService
  ) {
    this.authService
      .canModify(APP_MENU.booking.module, APP_MENU.booking.bookingHeader)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  }

  ngOnInit() {
    this.cols = [
      {
        field: 'status',
        display: 'custom-status',
        filter: 'dropdown',
        header: 'Label.Status',
        value: 'status'
      },
      {
        field: 'warehouseCode',
        display: 'string',
        filter: 'dropdown',
        header: 'Label.WarehouseCode',
        //width: '20rem'
      },
      {
        field: 'merchType',
        display: 'string',
        filter: 'string',
        header: 'Label.MerchType',
        //width: '15rem'
      },
      {
        field: 'internalKeyId',
        display: 'string',
        filter: 'string',
        header: 'Group Id'
      },
      {
        field: 'bookingId',
        display: 'string',
        filter: 'string',
        header: 'Label.BookingId',
      },
      {
        field: 'supCode',
        display: 'string',
        filter: 'string',
        header: 'Label.SupCode',
      },
      {
        field: 'supName',
        display: 'string',
        filter: 'string',
        header: 'Label.SupName',
      },      
      {
        field: 'bookingStart',
        display: 'datetime',
        filter: 'date',
        header: 'Label.BookingStart',
      },
      // {
      //   field: 'bookingEnd',
      //   display: 'datetime',
      //   filter: 'date',
      //   header: 'Label.BookingEnd',
      // },
      {
        field: 'totalPo',
        display: 'string',
        filter: 'number',
        header: 'Label.TotalPo',
      },
      {
        field: 'totalQty',
        display: 'string',
        filter: 'number',
        header: 'Label.TotalQty',
      },      
      {
        field: 'isDelay',
        display: 'bool',
        filter: 'bool',
        header: 'Delay',
      },
      {
        field: 'backHaul',
        display: 'bool',
        filter: 'bool',
        header: 'Back Haul',
      },
      {
        field: 'createDate',
        display: 'datetime',
        filter: 'date',
        header: 'Label.CreateDate',
      },
      {
        field: 'companyCode',
        display: 'string',
        filter: 'string',
        header: 'Label.CompanyCode',
      },
    ];

    this.lookup = {      
      status: [
        { label: 'All', value: null },
        { label: 'NEW', value: 'NEW'},
        { label: 'INTRANSIT', value: 'INTRANSIT'},
        { label: 'APPROVED', value: 'APPROVED'},
        { label: 'CHECKIN', value: 'CHECKIN'},
        { label: 'QUEUE', value: 'QUEUE'},
        { label: 'CALLTRUCK', value: 'CALLTRUCK'},
        { label: 'ONDOCK', value: 'ONDOCK'},
        { label: 'UNLOADING', value: 'UNLOADING'},
        { label: 'UNLOADED', value: 'UNLOADED'},
        { label: 'LEAVEDOOR', value: 'LEAVEDOOR'},
        { label: 'CHECKOUT', value: 'CHECKOUT'}
      ],
      warehouseCode: [{ label: 'All', value: null}],
    };

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        switchMap((sieve) => 
          this.bookingHeaderService.getPaged(sieve)
        ),
        untilDestroyed(this)
      )
      .subscribe({
        next: (paged) => {
          this.dataSource = paged.results;          
          let status: string[] = [];
          let warehouses: string[] = [];

          this.dataSource.forEach(d => {
            status.push(d.status);
            warehouses.push(d.warehouseCode);
          });
          
          status = status.filter((v,i,a)=>{
            return a.indexOf(v) == i;
          });

          warehouses = warehouses.filter((v,i,a)=>{
            return a.indexOf(v) == i;
          });

          this.lookup.warehouseCode = [];

          this.lookup.warehouseCode = [{ label: 'All', value: null}];

          this.lookup.warehouseCode = [
            ...this.lookup.warehouseCode,
            ...warehouses.map((x) => ({label: x, value:x})),
          ];


          this.dataSource.forEach((row)=>{          
            if(row.status == "NEW")
            {
              row.fontColor = "white";
              row.backgroundColor = "red";
            }
            else if(row.status == "APPROVED")
            {
              row.fontColor = "white";
              row.backgroundColor = "green";
            }
            else{
              row.fontColor = "white";
              row.backgroundColor = "var(--primary-400)";
            }            
            row.delayDisplay = row.isDelay ? 'Late' : 'ON TIME';
          });

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
  }

  ngOnDestroy() {
    if (this.ref) {
      this.ref.close();
    }
  }
}
