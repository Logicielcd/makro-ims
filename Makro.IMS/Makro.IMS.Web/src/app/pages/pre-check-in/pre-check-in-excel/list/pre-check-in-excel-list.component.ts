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
import * as FileSaver from 'file-saver';
import { UploadDialogComponent } from './upload-dialog/upload-dialog.component';
import { User } from '@core/models/account/user.model';
import { LanguageService } from '@core/services/language.service';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { TruckMasterService } from '@core/services/master/truckMaster.service';

@UntilDestroy()
@Component({
  templateUrl: './pre-check-in-excel-list.component.html',
})
export class PreCheckInExcelListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: BookingHeader[] = [];
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  user: User;

  ref: DynamicDialogRef;

  sieve: any;

  exportResult: any;

  trucks: TruckMaster[] = [];

  wopts: XLSX.WritingOptions = { bookType: 'xlsx', type: 'array' };

  canUpload: boolean = null;

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
    private authService: AuthService,
    private languageService: LanguageService,
    private truckService: TruckMasterService,
  ) 
  {
    this.authService
      .canModify(APP_MENU.booking.module, APP_MENU.booking.bookingHeader)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));

    this.user = this.authService.getUser();

    if(this.user.supGroup == null)
    {
      if(this.user.userType.toUpperCase() !== "SUP" && this.user.userType.toUpperCase() !== "SUPTRAN"){
        this.canUpload = true;
      }
      else{
        this.canUpload = false;
      }
    }
    else if(this.user.supGroup.isUpPreCheckin == "Y"){
      this.canUpload = true;
    }
    else{
      this.canUpload = false;
    }

  }

  ngOnInit() {
    this.cols = [
      {
        field: 'warehouseCode',
        display: 'string',
        filter: 'string',
        header: 'Label.WarehouseCode',
        width: '100rem',
      },
      {
        field: 'bookingId',
        display: 'string',
        filter: 'string',
        header: 'Label.BookingId',
        width: '100rem',
      },
      {
        field: 'supCode',
        display: 'string',
        filter: 'string',
        header: 'Label.SupCode',
        width: '100rem',
      },
      {
        field: 'supName',
        display: 'string',
        filter: 'string',
        header: 'Label.SupName',
        width: '250rem',
      },      
      {
        field: 'bookingStart',
        display: 'datetime',
        filter: 'date',
        header: 'Label.BookingStart',
        width: '100rem',
      },
      {
        field: 'bookingEnd',
        display: 'datetime',
        filter: 'date',
        header: 'Label.BookingEnd',
        width: '100rem',
      },
      {
        field: 'totalPo',
        display: 'string',
        filter: 'number',
        header: 'Label.TotalPo',
        width: '100rem',
      },
      {
        field: 'totalQty',
        display: 'string',
        filter: 'number',
        header: 'Label.TotalQty',
        width: '100rem',
      },
      // {
      //   field: 'status',
      //   display: 'string',
      //   filter: 'string',
      //   header: 'Label.Status',
      //   width: '100rem',
      // },
      {
        field: 'approveCondition',
        display: 'string',
        filter: 'string',
        header: 'Label.ApproveCondition',
        width: '100rem',
      },
    ];

    this.truckService.getAll().pipe(
      untilDestroyed(this)
    ).subscribe({next : (data => this.trucks = data)
      ,error: (error) => {
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
      }
    })

    this.pagingService.pagedData
      .pipe(
        tap(() => this.pagingService.setLoading(true)),
        tap((sieve) => (this.sieve = sieve)),
        switchMap((sieve) => 
          this.bookingHeaderService.getPreCheckInPaged(sieve),                      
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

  exportData(){
    this.bookingHeaderService.getPreCheckInExportPaged(this.sieve).pipe(untilDestroyed(this))
    .subscribe((result) => {       
      this.export(result.results);
    },(error)=>{
      this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
          });
    });
  }

  export(dataexport: any[]){        
    var wb = XLSX.utils.book_new();
    var wsPoData = [
      [ "Booking Id" ,"Warehouse","Booking Start","Booking End","Truck No","Truck Type","Truck License Plate Head","Truck License Plate Trail","Driver Name","Tel No","Line Account"]
    ];

    var bookingData = [] as BookingHeaderDto[];

    bookingData = dataexport;

    bookingData.forEach(booking => {
      booking.bookingTrucks = booking.bookingTrucks.sort((a,b) => b.remark.localeCompare(a.remark));
      var truckNo = 1;
      booking.bookingTrucks.forEach(bt => {
        for (let index = 0; index < bt.totalTruck; index++) {
          const element = bt.totalTruck;
          wsPoData.push([booking.bookingId,booking.warehouseCode,booking.bookingStart.toString(),booking.bookingEnd.toString(),
            truckNo.toString(), this.trucks.find(x=>x.internalTruckId == bt.internalTruckId).truckCode
          ]);  

          truckNo++;
        }
      });      
    });

    var wsPo = XLSX.utils.aoa_to_sheet(wsPoData);

    var wsPoColWidth = [{wch:20},{wch:15},{wch:25},{wch:25},{wch:15},{wch:15},{wch:30},{wch:30},{wch:25},{wch:15},{wch:15}];

    wsPo['!cols'] = wsPoColWidth;
    
    XLSX.utils.book_append_sheet(wb,wsPo,"TruckAssign");

    const excelBuffer: any = XLSX.write(wb, {
      bookType: 'xlsx',
      type: 'array',
    });

    const EXCEL_TYPE =
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([excelBuffer], {
      type: EXCEL_TYPE,
    });
    
    FileSaver.saveAs(
      data,
      'truckAssign' + '_export_' + new Date().getTime() + EXCEL_EXTENSION
    );

  }

  uploadData(){
    this.ref = this.dialogService.open(UploadDialogComponent, {
      header: 'Upload pre-check in file',
      width: '1280px',
      height: '800px',
      contentStyle: {"max-height": "1280", "overflow": "auto"},
      baseZIndex: 10000,
      // data: {
      //   warehouseCode: whse,
      //   bookingDate: this.bookingDate,
      //   supplierCode: this.supplierSelected,
      //   supplierName: this.suppliers.find(x=>x.supCode == this.supplierSelected).supName,          
      //   internalSupGroupId: this.suppliers.find(x=>x.supCode == this.supplierSelected).internalGroupId
      // }
    });

    this.ref.onClose.subscribe((ret: any) =>{
        if (ret)
        {
          
          // ret.forEach(po => {
          //   // this.addPoByPoList(po.po_Nbr);            
          // });
        }

    });
  }

}
