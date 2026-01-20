import { Component, OnInit, ViewChild,OnDestroy } from '@angular/core';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { NgForm } from '@angular/forms';
import { TablePageListComponent } from '@theme/components/table-page-list/table-page-list.component';
import { AuthService } from 'auth/auth.service';
import { User } from '@core/models/account/user.model';
import { ReportDockDoorControl } from '@core/models/report/report-dockdoorcontrol';
import { ReportService } from '@core/services/report/report.service';
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { Subject, interval, takeUntil } from 'rxjs';


@UntilDestroy()
@Component({
  templateUrl: './dockdoor-control.component.html',
})
export class DockDoorControlComponent implements OnInit,OnDestroy {
  private destroy$: Subject<void> = new Subject<void>();
  
  user : User;
  warehouses: Warehouse[];
  warehouseSelected: string = '';
  doorRange: string = '';

  visible: boolean = false;

  cols : any[];
  
  dockdoorControls: ReportDockDoorControl[];
  reportDate: Date;
  totalBooking: number;
  totalCheckIn: number;
  totalPending: number;
  totalInWarehouse: number;
  totalCheckOut: number;

  autoRefresh: boolean;
  refreshIntervalTime: number;
  refreshTime: Date;

  warehouseList: any[];

  @ViewChild('f') f: NgForm;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    UploadSuccess: 'Message.Notification.Upload',
  };

  constructor(private reportService: ReportService
    ,private warehouseService: WarehouseService    
    ,private authService: AuthService
    ) {      
      this.user = this.authService.getUser();
      this.autoRefresh = false;
      this.refreshIntervalTime = 60;
  }


  ngOnInit() {
    const urlParams = new URLSearchParams(window.location.search);
    this.warehouseSelected = urlParams.get('warehouse');
    
    this.cols = [
      {
        field: 'doorArea',
        display: 'string',
        filter: 'string',
        header: 'Operation',
      },
      {
        field: 'doorName',
        display: 'string',
        filter: 'string',
        header: 'Dock No',
      },
      {
        field: 'statusIcon',
        display: 'icon',
        filter: 'string',
        header: '',        
      },   
      {
        field: 'status',
        display: 'string',
        filter: 'string',
        header: 'Status',        
      },   
      {
        field: 'doorType',
        display: 'string',
        filter: 'string',
        header: 'Dock Type',        
      },      
      {
        field: 'truckType',
        display: 'string',
        filter: 'string',
        header: 'Truck Type',
      },
      {
        field: 'licensePlate',
        display: 'string',
        filter: 'string',
        header: 'License',
      },      
      {
        field: 'supName',
        display: 'string',
        filter: 'string',
        header: 'Sup Name',
      },
      {
        field: 'assignQueueTime',
        display: 'datetime',
        filter: 'string',
        header: 'Assign Queue',
      },
      {
        field: 'onDockTime',
        display: 'datetime',
        filter: 'string',
        header: 'On Dock',
      },
      {
        field: 'processTimeDisplay',
        display: 'string',
        filter: 'waitingTimeDisplay',
        header: 'On Process',
      },           
    ];

    this.doorRange = "";
    this.fetchData();
  }

  fetchData() : void{
    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe((d)=>{
      
      this.warehouses = d.filter(x=>x.warehouseWms !== null && x.warehouseWms !== undefined);
      
      this.warehouses = this.warehouses.filter(x=>this.user.userWarehouses.some(y=>y.warehouseCode == x.warehouseCode));

      this.warehouseList = Array.from(new Set(this.warehouses.map(x => x.warehouseCode)))
                          .map(warehouseCode => {
                            return { value: warehouseCode };
                          });

      this.warehouseList.sort((a, b) => a.value.localeCompare(b.value));

      // this.warehouses = this.warehouses.sort((a,b) => (a.warehouseCode > b.warehouseCode) ? 1 : ((b.warehouseCode > a.warehouseCode) ? -1 : 0));

      if (this.warehouseSelected == undefined || this.warehouseSelected == null){
        if (this.user.warehouseCode !== null && this.user.warehouseCode !== undefined){
          let whse = this.warehouses.filter(x=>x.warehouseCode == this.user.warehouseCode);
          this.warehouseSelected = whse[0].warehouseCode;          
        }
      }

      if(this.warehouseSelected !== null){
        this.getReportData();
      }

    });
  }
  
  saveParameter(){
    if(this.autoRefresh){
      // console.log('Auto-refresh component initialized');
      let sec = this.refreshIntervalTime * 1000;
      interval(sec) 
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        // console.log('Refreshing page...');
        this.refreshData();
      });
    }
    else{
      this.ngOnDestroy();      
    }
    this.getReportData();
  }

  ngOnDestroy() {
    // console.log('Refreshing page destroy');
    this.destroy$.next();
    this.destroy$.complete();
  }

  getReportData(){
    this.visible = false;
    this.refreshTime = new Date();

    this.reportService.getDockDoorControl(this.warehouseSelected,this.doorRange)
    .pipe(untilDestroyed(this))
    .subscribe((d) => {
      this.dockdoorControls = d;

      this.dockdoorControls.forEach(row => {
        
        row.processTimeDisplay = Math.floor(row.processTime/3600).toString().padStart(2,'0') + ':' + Math.floor((row.processTime%3600)/60).toString().padStart(2,'0');
        if(!row.supCode){
          row.status = 'Available';
          row.statusIcon = 'green';
        }
        else{
          row.status = 'Used';
          row.statusIcon = 'red';
        }
      });

    });

    // this.reportService.getSummaryBooking(this.warehouseSelected,this.companySelected)
    // .pipe(untilDestroyed(this))
    // .subscribe((d)=>{
    //   this.summaryBooking = d;
    //   this.summaryBooking.totalPending = this.summaryBooking.totalBooking - this.summaryBooking.totalCheckIn;
    // })  
  }

  exportExcel()
  {
    this.reportService.getDockDoorControl(this.warehouseSelected,this.doorRange)
      .pipe(untilDestroyed(this))
      .subscribe((d) => {
        this.dockdoorControls = d;
        this.export(this.dockdoorControls);
      });      
  }

  export(dataexport: ReportDockDoorControl[]){        
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
    
    XLSX.utils.book_append_sheet(wb,wsPo,"dockdoorcontrol");

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
      'dockdoorcontrol' + '_export_' + new Date().getTime() + EXCEL_EXTENSION
    );

  }

  getWaitingTimeFontColor(fieldData:string){
    if(fieldData !== undefined){      
      if( +fieldData.split(':')[1] > 30 || +fieldData.split(':')[0] > 0){
        return 'red'
      }
      else
      {
        return 'var(--text-color)'
      }
    }
    else{
      return 'var(--text-color)'
    }    
  }

  getIcon(fieldData:string){
    return fieldData;        
  }

  refreshData(){
    this.getReportData();
  }
}
