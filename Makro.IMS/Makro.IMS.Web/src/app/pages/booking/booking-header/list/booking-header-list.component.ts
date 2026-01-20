import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
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
  templateUrl: './booking-header-list.component.html',
})
export class BookingHeaderListComponent implements OnInit, OnDestroy {
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
    private warehouseService: WarehouseService,    
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
        header: 'Zone',
        //width: '15rem'
      },
      {
        field: 'revisionPrefix',
        display: 'string',
        filter: 'string',
        header: 'Booking Group'
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
        field: 'approveCondition',
        display: 'string',
        filter: 'string',
        header: 'Overcap',
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

    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe((data)=>{

      const whseList = data.filter(x=> this.authService.getUser().userWarehouses.some(y=>y.warehouseCode == x.warehouseCode));

      this.lookup = {
        status: [
          { label: 'All', value: null,fontColor: 'white',backgroundColor: 'green' },
          { label: 'NEW', value: 'NEW',fontColor: 'white',backgroundColor: 'orange'},
          { label: 'EDIT', value: 'EDIT',fontColor: 'black',backgroundColor: 'yellow'},
          { label: 'OVERCAP', value: 'OVERCAP',fontColor: 'white',backgroundColor: 'red'},
          { label: 'OVERCUTOFF', value: 'OVERCUTOFF',fontColor: 'black',backgroundColor: 'red'},
          { label: 'APPROVED', value: 'APPROVED',fontColor: 'white',backgroundColor: 'green'},
          { label: 'INTRANSIT', value: 'INTRANSIT',fontColor: 'white',backgroundColor: 'var(--primary-400)'},        
          { label: 'CHECKIN', value: 'CHECKIN',fontColor: 'white',backgroundColor: 'var(--primary-400)'},          
          { label: 'QUEUE', value: 'QUEUE',fontColor: 'white',backgroundColor: 'var(--primary-400)'},
          { label: 'CALLTRUCK', value: 'CALLTRUCK',fontColor: 'white',backgroundColor: 'var(--primary-400)'},
          { label: 'ONDOCK', value: 'ONDOCK',fontColor: 'white',backgroundColor: 'var(--primary-400)'},
          { label: 'UNLOADING', value: 'UNLOADING',fontColor: 'white',backgroundColor: 'var(--primary-400)'},
          { label: 'UNLOADED', value: 'UNLOADED',fontColor: 'white',backgroundColor: 'var(--primary-400)'},
          { label: 'LEAVEDOOR', value: 'LEAVEDOOR',fontColor: 'white',backgroundColor: 'var(--primary-400)'},
          { label: 'WAITINGDOC', value: 'WAITINGDOC',fontColor: 'white',backgroundColor: 'var(--primary-400)'},
          { label: 'SUBMITDOC', value: 'SUBMITDOC',fontColor: 'white',backgroundColor: 'var(--primary-400)'},        
          { label: 'CHECKOUT', value: 'CHECKOUT',fontColor: 'white',backgroundColor: 'var(--primary-400)'},
          { label: 'CANCEL', value: 'CANCEL',fontColor: 'white',backgroundColor: 'gray'},
        ],
        warehouseCode: [{ label: 'All', value: null}],
      };

      this.lookup.warehouseCode = [];

      this.lookup.warehouseCode = [{ label: 'All', value: null}];

      this.lookup.warehouseCode = [
        ...this.lookup.warehouseCode,
        ...whseList.map((x) => ({label: x.warehouseCode, value:x.warehouseCode})),
      ];
      
    });

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

          this.dataSource.forEach((row)=>{          
            if(row.status == "NEW")
            {
              row.fontColor = "white";
              row.backgroundColor = "orange";
            }
            if(row.status == "EDIT")
            {
              row.fontColor = "black";
              row.backgroundColor = "yellow";
            }
            else if(row.status == "OVERCAP")
            {
              row.fontColor = "white";
              row.backgroundColor = "red";
            }
            else if(row.status == "OVERCUTOFF")
              {
                row.fontColor = "black";
                row.backgroundColor = "red";
              }
            else if(row.status == "CANCEL")
            {
              row.fontColor = "white";
              row.backgroundColor = "gray";
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
