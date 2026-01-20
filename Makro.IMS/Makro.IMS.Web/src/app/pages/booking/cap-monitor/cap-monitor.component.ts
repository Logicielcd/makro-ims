import { Component, ElementRef, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseService } from '@core/services/master/warehouse.service';

import { NgForm } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';
import { ReportService } from '@core/services/report/report.service';
import { ReportSummaryBooking } from '@core/models/report/report-summarybooking';
import { ReportSlottimeBooking, ReportSlottimeBookingAllWhse } from '@core/models/report/report-slottimebooking';

import * as XLSX from 'xlsx';
import { OperationService } from '@core/services/master/operation.service';
import { Operation } from '@core/models/master/operation.model';
import { OperationCapacity } from '@core/models/master/operation-capacity.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { catchError, forkJoin, map, of } from 'rxjs';

@UntilDestroy()

@Component({  
  templateUrl: 'cap-monitor.component.html',
  styleUrls: ['cap-monitor.component.scss'],
  encapsulation: ViewEncapsulation.None,
})

export class CapMonitorComponent implements OnInit {

  user : User;
  warehouses: Warehouse[];
  warehouseList: any[];
  companies: any[];
  warehouseSelected: string = '';
  companySelected: string = '';

  operations: Operation[];
  operationCapacities: OperationCapacity[];
  
  summaryBooking: ReportSummaryBooking = {} as ReportSummaryBooking;

  reportDate: Date;

  visible: boolean = false;

  slottimeData: ReportSlottimeBookingAllWhse[];
  slottimeResult: any[];
  summaryData: any[];

  slottimeActualData: ReportSlottimeBooking[];
  slottimeActualResult: any[];
  summaryActualData: any[];

  slottimePendingData: ReportSlottimeBooking[];
  slottimePendingResult: any[];
  summaryPendingData: any[];

  slottimeHeader: any[];
  colspan: number;
  timeSlot: number;

  canModify: boolean = false;

  @ViewChild('f') f: NgForm;
  @ViewChild('report') table: ElementRef;

  constructor(private reportService: ReportService
    ,private warehouseService: WarehouseService    
    ,private operationService: OperationService
    ,private authService: AuthService
    ) {      
      
      this.authService
        .canModify(APP_MENU.booking.module, APP_MENU.booking.capMonitor)
        .pipe(untilDestroyed(this))
        .subscribe((granted) => (this.canModify = granted));

      this.user = this.authService.getUser();
  }

  ngOnInit() {
  //  const urlParams = new URLSearchParams(window.location.search);
    // this.warehouseSelected = urlParams.get('warehouse');
    // this.companySelected = urlParams.get('company');

    this.reportDate = new Date();

    this.fetchData();
  }

  fetchData() : void{
    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe((d)=>{
      this.warehouses = d.filter(x=>x.warehouseWms !== null && x.warehouseWms !== undefined);
      this.companies = [];

      this.warehouses = this.warehouses.filter(x=>x.capUom == 'CS');

      this.warehouseList = Array.from(new Set(this.warehouses.map(x => x.warehouseWms)))
                          .map(warehouseWms => {
                            return { value: warehouseWms };
                          });

      this.warehouseList.sort((a, b) => a.value.localeCompare(b.value));
      this.getBookingData();
    });
  }

  getBookingData() : void{      
      let timeStep = this.warehouses[0].timeIncreaseStep;
      this.timeSlot = 1440 / timeStep;
      this.visible = false;

      this.operationService.getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data)=>{
        this.operations = data;
      });

      const requests = this.warehouses.map(whse => 
        this.reportService.getSlottimeBooking(whse.warehouseWms,whse.companyCode, this.reportDate)
          .pipe(
            map(data => data.map(x => ({ 
              ...x, 
              slotTime: new Date(x.slotTime),
              warehouseCode: whse.warehouseCode, 
              groupData: whse.warehouseCode + '|' + x.merchType,
              totalRow: this.operations.filter(x=>x.warehouseCode === whse.warehouseCode).length *3
            }))),
            catchError(error => {
              console.error(`Error fetching warehouse ${whse.warehouseCode}:`, error);
              return of([]);  // คืนค่าว่างหากมี error เพื่อให้การทำงานต่อได้
            })
          )
      );
      
      forkJoin(requests)
      .pipe(untilDestroyed(this))
      .subscribe((allData) => {
        const combinedData = allData.flat();  // รวมข้อมูลทุก warehouse
        //console.log('Combined Slot Time Data:', combinedData);
        this.slottimeData = combinedData;
        this.slottimeResult = [];
        
        let merchTypes = Array.from(new Set(combinedData.map(item => item.groupData)));
  
        merchTypes.sort((a, b) => a.localeCompare(b));
        let mt = merchTypes[0];
        
        this.slottimeHeader = [];

        const colorList = [
          {fontColor: 'text-primary-600', bgColor: 'bg-primary-50'},
          {fontColor: 'text-green-600', bgColor: 'bg-green-50'},
          {fontColor: 'text-red-600', bgColor: 'bg-red-50'},
          {fontColor: 'text-cyan-600', bgColor: 'bg-cyan-50'},
          {fontColor: 'text-indigo-600', bgColor: 'bg-indigo-50'},
          {fontColor: 'text-teal-600', bgColor: 'bg-teal-50'},
          {fontColor: 'text-purple-600', bgColor: 'bg-purple-50'},
          {fontColor: 'text-pink-600', bgColor: 'bg-pink-50'},
        ];

        let curGroup = '';
        let rowGroup = 0;
        let fontColor = '#AAAAAA';
        let bgColor = '#AAAAAA';
        let bgIndex = 0;
        merchTypes.forEach((d)=>{  
          const optType = d.split('|')[1];
          const whseCode = d.split('|')[0];
          
          if(curGroup !== whseCode){
            curGroup = whseCode;
            rowGroup = 1;
            fontColor = colorList[bgIndex].fontColor;
            bgColor = colorList[bgIndex].bgColor;
            bgIndex++;
          }
          else{
            rowGroup++;
          }

          let detail = combinedData.filter(x=>x.groupData == d && x.dataType == 'Capacity');
          let sumQty: number = 0.00;
          detail.forEach(item => {
            sumQty += item.quantity;

            if(item.groupData == mt)
              this.slottimeHeader.push( {headerText: item.slotTime.getHours().toString().padStart(2,'0') 
              + ':' + item.slotTime.getMinutes().toString().padStart(2,'0') });
          });
  
          this.colspan = this.slottimeHeader.length + 2;

          let sData = {merchType: optType,
            dataType: "Capacity",
            details: detail,
            totalQty: sumQty,
            uom: "Kg",
            cap: this.operations.filter(x=>x.operationName == optType)[0].capacity,
            warehouseCode: whseCode,
            totalRow: detail[0].totalRow,
            rowGroup: rowGroup,
            fontColor: fontColor,
            bgColor: bgColor
          };
  
          this.slottimeResult.push(sData);  

          detail = combinedData.filter(x=>x.groupData == d && x.dataType == 'Booking');
          sumQty = 0.00;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          sData = {merchType: optType,
            dataType: "Booking",
            details: detail,
            totalQty: sumQty,
            uom: "Kg",
            cap: this.operations.filter(x=>x.operationName == optType)[0].capacity,
            warehouseCode: whseCode,
            totalRow: detail[0].totalRow,
            rowGroup: rowGroup,
            fontColor: fontColor,
            bgColor: bgColor
          };
  
          this.slottimeResult.push(sData);      
  
          detail = combinedData.filter(x=>x.groupData == d && x.dataType == 'Truck');
  
          sumQty = 0;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          sData = {merchType: optType,
            dataType: "Truck",
            details: detail,
            totalQty: sumQty,
            uom: "Truck",
            cap: this.operations.filter(x=>x.operationName == optType)[0].capacity,
            warehouseCode: whseCode,
            totalRow: detail[0].totalRow,
            rowGroup: rowGroup,
            fontColor: fontColor,
            bgColor: bgColor
          };
  
          this.slottimeResult.push(sData);  
          
        })
          
      });

  }

  public onWhseChange(event: any):void {
    this.getBookingData();
  }

  exportExcel()
  {
    const ws: XLSX.WorkSheet=XLSX.utils.table_to_sheet(this.table.nativeElement);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
    
    /* save to file */
    XLSX.writeFile(wb, 'report_slottime_booking.xlsx');
    
  }
  
  adjustColor(hex: string, rstep: number,gstep: number,bstep:number): string {
    let r = parseInt(hex.substring(1, 3), 16);
    let g = parseInt(hex.substring(3, 5), 16);
    let b = parseInt(hex.substring(5, 7), 16);
  
    r = Math.max(0, r - rstep);
    g = Math.max(0, g - gstep);
    b = Math.max(0, b - bstep);
  
    return `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b.toString(16).padStart(2, '0')}`;
  }

}
