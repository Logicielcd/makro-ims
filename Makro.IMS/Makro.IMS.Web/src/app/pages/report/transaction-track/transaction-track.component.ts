import { Component, ElementRef, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseService } from '@core/services/master/warehouse.service';

import { NgForm } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';
import { ReportService } from '@core/services/report/report.service';
import { ReportTransactionTrack } from '@core/models/report/report-transactiontrack';
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { PagingService } from '@core/services/paging.service';
import { TablePageListComponent } from '@theme/components/table-page-list/table-page-list.component';

import { Table } from 'primeng/table';

@UntilDestroy()

@Component({  
  templateUrl: 'transaction-track.component.html',
  styleUrls: ['transaction-track.component.scss'],
})

export class TransactionTrackComponent implements OnInit {

  user : User;
  warehouses: Warehouse[];
  companies: any[];
  warehouseSelected: string = '';
  companySelected: string = '';
  warehouseMaster: Warehouse[];

  cols : any[];
  
  transactionTrackings: ReportTransactionTrack[] = {} as ReportTransactionTrack[];
  reportDate: Date;
  totalBooking: number;
  totalCheckIn: number;
  totalPending: number;
  totalInWarehouse: number;
  totalCheckOut: number;
  defaultWidth: string;
  visible: boolean;

  rangeDates: Date[];

  @ViewChild('f') f: NgForm;
  @ViewChild(TablePageListComponent) tableComp: TablePageListComponent;
  @ViewChild('report') table: ElementRef;


  constructor(private reportService: ReportService
    ,private warehouseService: WarehouseService    
    ,private authService: AuthService
    ,private pagingService: PagingService,
    ) {      
      this.user = this.authService.getUser();
  }

  ngOnInit() {
    const urlParams = new URLSearchParams(window.location.search);
    this.warehouseSelected = urlParams.get('warehouse');
    this.companySelected = urlParams.get('company');
    this.defaultWidth = '50rem';

    this.cols = [
      {
        field: 'warehouseCode',
        display: 'string',
        filter: 'string',
        header: 'Warehouse Code',        
      },
      {
        field: 'companyCode',
        display: 'string',
        filter: 'string',
        header: 'Company Code',        
      },      
      {
        field: 'bookingDate',
        display: 'date',
        filter: 'string',
        header: 'Booking Date',
      },
      {
        field: 'bookingId',
        display: 'string',
        filter: 'string',
        header: 'Booking Id',
        width: '100rem',
      },
      {
        field: 'backHaul',
        display: 'string',
        filter: 'string',
        header: 'Truck Group',
      },
      {
        field: 'totalPoBooking',
        display: 'string',
        filter: 'string',
        header: 'Total PO Booking',
      },
      {
        field: 'poBooking',
        display: 'string',
        filter: 'string',
        header: 'PO Booking',
      },
      {
        field: 'totalPoChecking',
        display: 'string',
        filter: 'string',
        header: 'Total PO Checking',
      },
      {
        field: 'poChecking',
        display: 'string',
        filter: 'string',
        header: 'PO Checking',
      },
      {
        field: 'totalPoConfirm',
        display: 'string',
        filter: 'string',
        header: 'Total PO Confirm',
      },
      {
        field: 'poConfirm',
        display: 'string',
        filter: 'string',
        header: 'PO Confirm',
      },
      {
        field: 'supCode',
        display: 'string',
        filter: 'string',
        header: 'Sup Code',
      },
      {
        field: 'supName',
        display: 'string',
        filter: 'string',
        header: 'Sup Name',
      },
      {
        field: 'userId',
        display: 'string',
        filter: 'string',
        header: 'User Id',
      },
      {
        field: 'userType',
        display: 'string',
        filter: 'string',
        header: 'Role',
      },      
      {
        field: 'totalTruck',
        display: 'string',
        filter: 'string',
        header: 'Total Truck',
      },
      {
        field: 'totalWeight',
        display: 'decimal',
        filter: 'string',
        header: 'Total Kg or CS',
      },
      {
        field: 'licensePlate',
        display: 'string',
        filter: 'string',
        header: 'Truck ID Head',
      },
      {
        field: 'licensePlate2',
        display: 'string',
        filter: 'string',
        header: 'Truck ID Trail',
      },
      {
        field: 'driverName',
        display: 'string',
        filter: 'string',
        header: 'Driver Name',
      },
      {
        field: 'truckCode',
        display: 'string',
        filter: 'string',
        header: 'Truck Type',
      },
      {
        field: 'door',
        display: 'string',
        filter: 'string',
        header: 'Dock Door',
      },
      {
        field: 'merchType',
        display: 'string',
        filter: 'string',
        header: 'Zone',
      },
      {
        field: 'status',
        display: 'string',
        filter: 'string',
        header: 'Status',
        width: '100rem',
      },   
      {
        field: 'createDate',
        display: 'datetime',
        filter: 'string',
        header: 'Create Booking Date',
      },
      {
        field: 'checkinTime',
        display: 'datetime',
        filter: 'string',
        header: 'Create Truck',
      },
      {
        field: 'bookingDate',
        display: 'datetime',
        filter: 'string',        
        header: 'Select Book Date',
      },
      {
        field: 'arrivedTime',
        display: 'datetime',
        filter: 'string',
        header: 'Check In Time',
      },
      {
        field: 'qWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting Q. ==>',
      },
      {
        field: 'assignQueueTime',
        display: 'datetime',
        filter: 'string',
        header: 'Assign Queue Time',
      },
      {
        field: 'callWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting Call==>',
      },
      {
        field: 'callTruckTime',
        display: 'datetime',
        filter: 'string',
        header: 'Assign Truck Time',
      },
      {
        field: 'onDockWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting on dock ==>',
      },
      {
        field: 'onDockTime',
        display: 'datetime',
        filter: 'string',
        header: 'On Dock Time',
      },
      {
        field: 'unloadWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting start unload ==>',
      },
      {
        field: 'startUnloadTime',
        display: 'datetime',
        filter: 'string',
        header: 'Start Unload Time',
      },
      {
        field: 'unloadedWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting unload ==>',
      },
      {
        field: 'finishUnloadTime',
        display: 'datetime',
        filter: 'string',
        header: 'End unload Time',
      },
      {
        field: 'leaveWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting Leave door ==>',
      },
      {
        field: 'departureTime',
        display: 'datetime',
        filter: 'string',
        header: 'Leave Dock Time',
      },
      {
        field: 'docWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting Doc ==>',
      },
      {
        field: 'waitingDocumentTime',
        display: 'datetime',
        filter: 'string',
        header: 'Waiting Doc Time',
      },
      {
        field: 'submitWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting Submit Doc ==>',
      },
      {
        field: 'submitDocTime',
        display: 'datetime',
        filter: 'string',
        header: 'Confirm Doc Time',
      },
      {
        field: 'checkoutWaitingDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        colorcheck: 'Y',
        header: '<== Waiting checkout ==>',
      },
      {
        field: 'checkoutTime',
        display: 'datetime',
        filter: 'string',
        header: 'Check Out Time',
      },
      {
        field: 'totalTimeDisplay',
        display: 'string',
        filter: 'string',
        color: '',
        header: 'Total Time',
      },    
      {
        field: 'remark',
        display: 'string',
        filter: 'string',
        color: '',
        header: 'Remark',
      },      
      {
        field: 'remarkBooking',
        display: 'string',
        filter: 'string',
        color: '',
        header: 'Remark Booking',
      },      
      {
        field: 'remarkDelay',
        display: 'string',
        filter: 'string',
        color: '',
        header: 'Remark Delay',
      },      
    ];

    this.rangeDates = [new Date(),new Date()];
    this.fetchData();
  }

  fetchData() : void{
    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe((d)=>{
      this.warehouseMaster = d.filter(x=>x.warehouseWms !== null && x.warehouseWms !== undefined);
      this.warehouses = d.filter(x=>x.warehouseWms !== null && x.warehouseWms !== undefined);
      
      this.companies = [];

      let coms = [];

      this.warehouses.forEach(whse => {
        coms.push({companyCode: whse.companyCode});
      });

      this.companies = Array.from(new Map(coms.map(item=>[item.companyCode,item])).values());

      //console.log(this.companies);

      // this.companies = this.companies.filter((v,i,a)=>{
      //   return a.indexOf(v) === i;
      // });

      if (this.warehouseSelected == undefined || this.warehouseSelected == null){
        if (this.user.warehouseCode !== null && this.user.warehouseCode !== undefined){
          let whse = this.warehouses.filter(x=>x.warehouseCode == this.user.warehouseCode);
          this.warehouseSelected = whse[0].warehouseWms;
          this.companySelected = whse[0].companyCode;
        }
      }

      if(this.warehouseSelected !== null && this.companySelected !== null){
        this.getReport();
      }

    });

  }

  getReport(){
    this.reportService.getTransactionTrack(this.warehouseSelected,this.companySelected,this.rangeDates[0],this.rangeDates[1])
    .pipe(untilDestroyed(this))
    .subscribe((d) => {
      this.transactionTrackings = d;

      this.transactionTrackings.forEach(row => {
        row.qWaitingDisplay = Math.floor(row.qWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.qWaiting%3600)/60).toString().padStart(2,'0');
        row.callWaitingDisplay = Math.floor(row.callWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.callWaiting%3600)/60).toString().padStart(2,'0');
        row.onDockWaitingDisplay = Math.floor(row.onDockWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.onDockWaiting%3600)/60).toString().padStart(2,'0');
        row.unloadWaitingDisplay = Math.floor(row.unloadWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.unloadWaiting%3600)/60).toString().padStart(2,'0');
        row.unloadedWaitingDisplay = Math.floor(row.unloadedWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.unloadedWaiting%3600)/60).toString().padStart(2,'0');
        row.leaveWaitingDisplay = Math.floor(row.leaveWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.leaveWaiting%3600)/60).toString().padStart(2,'0');
        row.docWaitingDisplay = Math.floor(row.docWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.docWaiting%3600)/60).toString().padStart(2,'0');
        row.submitWaitingDisplay = Math.floor(row.submitWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.submitWaiting%3600)/60).toString().padStart(2,'0');
        row.checkoutWaitingDisplay = Math.floor(row.checkoutWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.checkoutWaiting%3600)/60).toString().padStart(2,'0');
        row.totalTimeDisplay = Math.floor(row.totalTime/3600).toString().padStart(2,'0') + ':' + Math.floor((row.totalTime%3600)/60).toString().padStart(2,'0');
      });

    });
  }


  exportExcel()
  {
    this.exportData();
    // const ws: XLSX.WorkSheet=XLSX.utils.table_to_sheet(this.table.nativeElement);
    // const wb: XLSX.WorkBook = XLSX.utils.book_new();
    // XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
    
    // /* save to file */
    // XLSX.writeFile(wb, 'report_transaction_tracking.xlsx');
    
  }

  exportData(){
    this.reportService.getTransactionTrack(this.warehouseSelected,this.companySelected,this.rangeDates[0],this.rangeDates[1])
    .pipe(untilDestroyed(this))
    .subscribe((d) => {
      d.forEach(row => {
        row.qWaitingDisplay = Math.floor(row.qWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.qWaiting%3600)/60).toString().padStart(2,'0');
        row.callWaitingDisplay = Math.floor(row.callWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.callWaiting%3600)/60).toString().padStart(2,'0');
        row.onDockWaitingDisplay = Math.floor(row.onDockWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.onDockWaiting%3600)/60).toString().padStart(2,'0');
        row.unloadWaitingDisplay = Math.floor(row.unloadWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.unloadWaiting%3600)/60).toString().padStart(2,'0');
        row.unloadedWaitingDisplay = Math.floor(row.unloadedWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.unloadedWaiting%3600)/60).toString().padStart(2,'0');
        row.leaveWaitingDisplay = Math.floor(row.leaveWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.leaveWaiting%3600)/60).toString().padStart(2,'0');
        row.docWaitingDisplay = Math.floor(row.docWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.docWaiting%3600)/60).toString().padStart(2,'0');
        row.submitWaitingDisplay = Math.floor(row.submitWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.submitWaiting%3600)/60).toString().padStart(2,'0');
        row.checkoutWaitingDisplay = Math.floor(row.checkoutWaiting/3600).toString().padStart(2,'0') + ':' + Math.floor((row.checkoutWaiting%3600)/60).toString().padStart(2,'0');
        row.totalTimeDisplay = Math.floor(row.totalTime/3600).toString().padStart(2,'0') + ':' + Math.floor((row.totalTime%3600)/60).toString().padStart(2,'0');
      });
      this.export(d);
    // },(error)=>{
    //   this.msgs = [];
    //       error.Messages.forEach((msg: any) => {
    //         this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
    //       });
    // }
    });
  }

  export(dataexport: ReportTransactionTrack[]){        
    var wb = XLSX.utils.book_new();
    var wsData = <any[]>[] ;
    var wsCols = <any[]>[];
    var wsDbCols = <any[]>[];
    
    this.cols.forEach(col => {
      wsCols.push(col.header);
      wsDbCols.push(col.field);
    })

    wsData.push(wsCols);

    // 1. สร้างข้อมูลใหม่ที่สอดคล้องกับหัวของคอลัมน์
    const mappedData = dataexport.map((item) => {
      const newItem: any = {};
      wsCols.forEach((header, index) => {        
        newItem[header] =
          item[
            this.cols[index].field
          ];
      });
      return newItem;
    });
    var wsPo = XLSX.utils.json_to_sheet(mappedData);
    
    XLSX.utils.book_append_sheet(wb,wsPo,"TransactionTracking");

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
      'transactiontracking' + '_export_' + new Date().getTime() + EXCEL_EXTENSION
    );

  }

  getWaitingTimeFontColor(fieldData:string){
    if( +fieldData.split(':')[0] > 0 || +fieldData.split(':')[1] > 30 ){
      return 'red'
    }
    else
    {
      return 'var(--text-color)'
    }
  }

  
  clear(table: Table) {
    table.clear();
  }

}
