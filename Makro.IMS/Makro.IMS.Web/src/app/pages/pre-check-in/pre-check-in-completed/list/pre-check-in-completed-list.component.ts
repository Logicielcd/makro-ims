import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BookingHeader, BookingHeaderDto } from '@core/models/booking/booking-header.model';
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
import * as XLSX from 'xlsx';
import { ReportDialogComponent } from '../report/report-dialog.component';

@UntilDestroy()
@Component({
  templateUrl: './pre-check-in-completed-list.component.html',
})
export class PreCheckInCompletedListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: BookingHeader[] = [];
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  ref: DynamicDialogRef;

  sieve: any;
  lookup: any;

  exportResult: any;

  wopts: XLSX.WritingOptions = { bookType: 'xlsx', type: 'array' };

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
      .canModify(APP_MENU.precheckin.module, APP_MENU.precheckin.preCheckInCompleted)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  }

  ngOnInit() {
//    this.canModify = true;

    this.cols = [
      {
        field: 'internalHeaderKey',
        display: 'print',
        header: '',
        width: '60px',
      },
      {
        field: 'warehouseCode',
        display: 'string',
        filter: 'string',
        header: 'Label.WarehouseCode',
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
        field: 'bookingEnd',
        display: 'datetime',
        filter: 'date',
        header: 'Label.BookingEnd',
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
      // {
      //   field: 'status',
      //   display: 'custom-status',
      //   filter: 'dropdown',
      //   // display: 'string',
      //   // filter: 'string',
      //   header: 'Label.Status',
      // },
      // {
      //   field: 'approveCondition',
      //   display: 'string',
      //   filter: 'string',
      //   header: 'Label.ApproveCondition',
      // },
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
        tap((sieve) => (this.sieve = sieve)),
        switchMap((sieve) => 
          this.bookingHeaderService.getPreCheckInCompletedPaged(sieve),                      
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

  openReport(){
    this.ref = this.dialogService.open(ReportDialogComponent, {
      header: 'Gate Pass',
      width: '22cm',
      height: 'auto',      
      contentStyle: {"min-height": "29.7cm","width": "22cm", "overflow": "auto"},
      baseZIndex: 10000,
    });
  }
 
  onPrintData(data: any){
    this.ref = this.dialogService.open(ReportDialogComponent, {
      header: 'Gate Pass',
      width: '23cm',
      height: 'auto',      
      contentStyle: {"min-height": "29.7cm","width": "23cm", "overflow": "auto"},
      baseZIndex: 10000,
      data: {
        bookingId: data,
      }
    });
  };

}
