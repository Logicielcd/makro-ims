import { Component, OnInit, ViewChild } from '@angular/core';

import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseService } from '@core/services/master/warehouse.service';

import { NgForm } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';
import { ReportService } from '@core/services/report/report.service';
import { ReportSummaryBooking } from '@core/models/report/report-summarybooking';
import { ReportTruckStatus } from '@core/models/report/report-truckstatus';
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { createSubjectOnTheInstance } from '@ngneat/until-destroy/lib/internals';

@UntilDestroy()

@Component({  
  templateUrl: 'truck-status.component.html',
  styleUrls: ['truck-status.component.scss'],
})

export class TruckStatusComponent implements OnInit {

  user : User;
  warehouses: Warehouse[];
  companies: any[];
  warehouseList: any[];
  warehouseSelected: string = '';
  companySelected: string = '';
  rangeDates: Date[];
  dateFrom: Date;
  dateTo: Date;
  visible: boolean = false;
  cols : any[];
  
  summaryBooking: ReportSummaryBooking = {} as ReportSummaryBooking;
  truckStatus: ReportTruckStatus[];
  reportDate: Date;
  totalOvercap: number;
  totalOvercutoff: number;
  totalNew: number;
  totalBooking: number;
  totalCheckIn: number;
  totalPending: number;
  totalInWarehouse: number;
  totalCheckOut: number;
  totalApproved: number;
  totalEdit: number;

  @ViewChild('f') f: NgForm;

  constructor(private reportService: ReportService
    ,private warehouseService: WarehouseService    
    ,private authService: AuthService
    ) {
      this.user = this.authService.getUser();
  }

  ngOnInit() {
    const urlParams = new URLSearchParams(window.location.search);
    this.warehouseSelected = urlParams.get('warehouse');
    this.companySelected = urlParams.get('company');

    this.cols = [
      {
        field: 'status',
        display: 'string',
        filter: 'string',
        header: 'Status',        
      },   
      {
        field: 'rowNo',
        display: 'string',
        filter: 'string',
        header: 'Row No.',        
      },            
      {
        field: 'slotBooking',
        display: 'string',
        filter: 'string',
        header: 'Delivery Date',
      },
      {
        field: 'bookingId',
        display: 'string',
        filter: 'string',
        header: 'Booking Id',
      },
      {
        field: 'licensePlate',
        display: 'string',
        filter: 'string',
        header: 'Truck ID',
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
        field: 'merchType',
        display: 'string',
        filter: 'string',
        header: 'Zone',
      },
      {
        field: 'totalWeight',
        display: 'string',
        filter: 'string',
        header: 'Total Kg or CS',
      },
      {
        field: 'door',
        display: 'string',
        filter: 'string',
        header: 'Dock Door',
      },
      {
        field: 'queueSeq',
        display: 'string',
        filter: 'string',
        header: 'Queue',
      },
      
      
    ];

    this.reportDate = new Date();
    this.totalOvercap = 0;
    this.totalNew = 0;
    this.totalBooking = 0;
    this.totalCheckIn = 0;
    this.totalPending = 0;
    this.totalInWarehouse = 0;
    this.totalCheckOut = 0;
    this.totalApproved = 0;
    this.totalOvercutoff = 0;
    this.totalEdit = 0;

    this.rangeDates = [new Date(),new Date()];
    this.dateFrom = new Date();
    this.dateTo = new Date();

    this.fetchData();
  }

  fetchData() : void{
    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe((d)=>{
      this.warehouses = d.filter(x=>x.warehouseWms !== null && x.warehouseWms !== undefined);
      this.companies = [];

      this.warehouses = this.warehouses.filter(x=>this.user.userWarehouses.some(y=>y.warehouseCode == x.warehouseCode));

      this.warehouseList = Array.from(new Set(this.warehouses.map(x => x.warehouseWms)))
                          .map(warehouseWms => {
                            return { value: warehouseWms };
                          });

      this.warehouseList.sort((a, b) => a.value.localeCompare(b.value));

      if (this.warehouseSelected == undefined || this.warehouseSelected == null){
        if (this.user.warehouseCode !== null && this.user.warehouseCode !== undefined){
          let whse = this.warehouses.filter(x=>x.warehouseCode == this.user.warehouseCode);
          this.warehouseSelected = whse[0].warehouseWms;

          this.companies = [];

          this.warehouses.filter(x=>x.warehouseWms == whse[0].warehouseWms).forEach(whse => {
            this.companies.push({companyCode: whse.companyCode});
          });

          this.companySelected = whse[0].companyCode;
        }
      }

      if(this.warehouseSelected !== null && this.companySelected !== null){
        this.getBookingData();
      }

    });

  }

  getBookingData(){
      this.visible = false;

      this.reportService.getTruckStatus(this.warehouseSelected,this.companySelected,this.dateFrom,this.dateTo)
      .pipe(untilDestroyed(this))
      .subscribe((d) => {
        this.truckStatus = d;

        this.totalOvercap = this.truckStatus.filter(x=>x.status.includes('00')).length;
        this.totalOvercutoff = this.truckStatus.filter(x=>x.status.includes('01')).length;
        this.totalNew = this.truckStatus.filter(x=>x.status.includes('02')).length;
        this.totalEdit = this.truckStatus.filter(x=>x.status.includes('03')).length;
        this.totalApproved = this.truckStatus.filter(x=>x.status.includes('04')).length;
        this.totalBooking = this.truckStatus.length - this.totalOvercap;
        this.totalPending = this.truckStatus.filter(x=>x.status.includes('05')).length;
        this.totalCheckIn = this.truckStatus.filter(x=>x.status.includes('06')).length;        
        this.totalCheckOut = this.truckStatus.filter(x=>x.status.includes('15')).length;
        this.totalInWarehouse = this.truckStatus.length - this.totalOvercap - this.totalOvercutoff - this.totalEdit - this.totalNew - this.totalPending - this.totalCheckIn - this.totalCheckOut - this.totalApproved;
      });

      // this.reportService.getSummaryBooking(this.warehouseSelected,this.companySelected)
      // .pipe(untilDestroyed(this))
      // .subscribe((d)=>{
      //   this.summaryBooking = d;
      //   this.summaryBooking.totalPending = this.summaryBooking.totalBooking - this.summaryBooking.totalCheckIn;
      // })
    
  }

  getWaitingTimeFontColor(fieldData:string){
    if( +fieldData.split(':')[0] > 0 ){
      return 'red'
    }
    else
    {
      return 'var(--text-color)'
    }
  }

  exportExcel()
  {
    this.reportService.getTruckStatus(this.warehouseSelected,this.companySelected,this.dateFrom,this.dateTo)
      .pipe(untilDestroyed(this))
      .subscribe((d) => {
        this.truckStatus = d;
        this.export(this.truckStatus);
      });      
  }

  export(dataexport: ReportTruckStatus[]){        
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
    
    XLSX.utils.book_append_sheet(wb,wsPo,"truckStatus");

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
      'truckstatus' + '_export_' + new Date().getTime() + EXCEL_EXTENSION
    );

  }

  onWarehouseSelected(event: any): void {
    this.companySelected = null;
    this.companies = [];
    this.warehouses.filter(x=>x.warehouseWms == event.value).forEach(whse => {
      this.companies.push({companyCode: whse.companyCode});
    });
  }

}
