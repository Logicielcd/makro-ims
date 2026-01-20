import { Component, ElementRef, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseService } from '@core/services/master/warehouse.service';

import { NgForm } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';
import { ReportService } from '@core/services/report/report.service';
import { ReportSummaryBooking } from '@core/models/report/report-summarybooking';
import { ReportSlottimeBooking } from '@core/models/report/report-slottimebooking';

import * as XLSX from 'xlsx';
import { OperationService } from '@core/services/master/operation.service';
import { Operation } from '@core/models/master/operation.model';
import { OperationCapacity } from '@core/models/master/operation-capacity.model';

@UntilDestroy()

@Component({  
  templateUrl: 'slottime-booking.component.html',
  styleUrls: ['slottime-booking.component.scss'],
  encapsulation: ViewEncapsulation.None,
})

export class SlottimeBookingComponent implements OnInit {

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

  slottimeData: ReportSlottimeBooking[];
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

  @ViewChild('f') f: NgForm;
  @ViewChild('report') table: ElementRef;

  constructor(private reportService: ReportService
    ,private warehouseService: WarehouseService    
    ,private operationService: OperationService
    ,private authService: AuthService
    ) {      
      this.user = this.authService.getUser();      
  }

  ngOnInit() {
    const urlParams = new URLSearchParams(window.location.search);
    this.warehouseSelected = urlParams.get('warehouse');
    this.companySelected = urlParams.get('company');

    this.reportDate = new Date();

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

  getBookingData() : void{
      let whse = this.warehouses.find(x=>x.warehouseWms == this.warehouseSelected && x.companyCode == this.companySelected);
      let timeStep = whse.timeIncreaseStep; // this.warehouses.find(x=>x.warehouseWms == this.warehouseSelected && x.companyCode == this.companySelected).timeIncreaseStep;
      this.timeSlot = 1440 / timeStep;
      // console.log(timeStep);
      // console.log(this.timeSlot);
      this.visible = false;

      let displayUom = whse.capUom === 'CS' ? 'CS' : 'Kg';

      this.operationService.getByWarehouse(this.warehouses.filter(x=>x.warehouseWms == this.warehouseSelected && x.companyCode == this.companySelected)[0].warehouseCode)
      .pipe(untilDestroyed(this))
      .subscribe((data)=>{
        this.operations = data;
      });

      this.reportService.getSlottimeBooking(this.warehouseSelected,this.companySelected,this.reportDate)
      .pipe(untilDestroyed(this))
      .subscribe((data)=>{

        data.forEach(x => x.slotTime = new Date(x.slotTime));

        this.slottimeData = data;
        this.slottimeResult = [];
        let merchTypes = Array.from(new Set(data.map(item => item.merchType)));
  
        merchTypes.sort((a, b) => a.localeCompare(b));
        
        let mt = merchTypes[0];
        
        this.slottimeHeader = [];

        merchTypes.forEach((d)=>{
            
          let detail = data.filter(x=>x.merchType == d && x.dataType == 'Capacity');
  
          let sumQty: number = 0.00;
  
          detail.forEach(item => {
            sumQty += item.quantity;

            if(item.merchType == mt)
              this.slottimeHeader.push( {headerText: item.slotTime.getHours().toString().padStart(2,'0') 
              + ':' + item.slotTime.getMinutes().toString().padStart(2,'0') });
          });
  
          this.colspan = this.slottimeHeader.length + 2;

          let sData = {merchType: d,
            dataType: "Capacity",
            details: detail,
            totalQty: sumQty,
            uom: displayUom,
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimeResult.push(sData);  

          detail = data.filter(x=>x.merchType == d && x.dataType == 'Booking');
          sumQty = 0.00;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          
          sData = {merchType: d,
            dataType: "Booking",
            details: detail,
            totalQty: sumQty,
            uom: displayUom,
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimeResult.push(sData);      
  
          detail = data.filter(x=>x.merchType == d && x.dataType == 'Truck');
  
          sumQty = 0;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          sData = {merchType: d,
            dataType: "Truck",
            details: detail,
            totalQty: sumQty,
            uom: "Truck",
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimeResult.push(sData);  

          
        })
  
        // summary truck per hour
        this.summaryData = [];
  
        for (let index = 0; index < this.timeSlot; index++) {
  
          let cSlot: Date;
          let cSum: number;
          
          // cSlot = this.slottimeResult[index].slottime;
          cSum = 0.00;
          
          merchTypes.forEach((d)=>{          
            let re = this.slottimeResult.filter(x=>x.dataType == 'Truck' && x.merchType == d);
            cSum += re[0].details[index].quantity;
          });
  
          this.summaryData.push({slottime: index,quantity: cSum})
        }
  
      });
  
      this.reportService.getSlottimeBookingActual(this.warehouseSelected,this.companySelected,this.reportDate)
      .pipe(untilDestroyed(this))
      .subscribe((data)=>{
        this.slottimeActualData = data;
        this.slottimeActualResult = [];
        let merchTypes = Array.from(new Set(data.map(item => item.merchType)));
  
        merchTypes.sort((a, b) => a.localeCompare(b));
        
        merchTypes.forEach((d)=>{

          let detail = data.filter(x=>x.merchType == d && x.dataType == 'Capacity');
  
          let sumQty: number = 0.00;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          let sData = {merchType: d,
            dataType: "Capacity",
            details: detail,
            totalQty: sumQty,
            uom: displayUom,
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimeActualResult.push(sData);  

          detail = data.filter(x=>x.merchType == d && x.dataType == 'Booking');
          sumQty = 0;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          sData = {merchType: d,
            dataType: "Booking",
            details: detail,
            totalQty: sumQty,
            uom: displayUom,
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimeActualResult.push(sData);      
  
          detail = data.filter(x=>x.merchType == d && x.dataType == 'Truck');
  
          sumQty = 0;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          sData = {merchType: d,
            dataType: "Truck",
            details: detail,
            totalQty: sumQty,
            uom: "Truck",
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimeActualResult.push(sData);  
        })
  
        // summary truck per hour
        this.summaryActualData = [];
  
        for (let index = 0; index < this.timeSlot; index++) {
  
          let cSlot: Date;
          let cSum: number;
          
          // cSlot = this.slottimeResult[index].slottime;
          cSum = 0;
          
          merchTypes.forEach((d)=>{          
            let re = this.slottimeActualResult.filter(x=>x.dataType == 'Truck' && x.merchType == d);
            cSum += re[0].details[index].quantity;
          });
  
          this.summaryActualData.push({slottime: index,quantity: cSum})
        }
  
      });

      this.reportService.getSlottimeBookingPending(this.warehouseSelected,this.companySelected,this.reportDate)
      .pipe(untilDestroyed(this))
      .subscribe((data)=>{
        this.slottimePendingData = data;
        this.slottimePendingResult = [];
        let merchTypes = Array.from(new Set(data.map(item => item.merchType)));
        merchTypes.sort((a, b) => a.localeCompare(b));
        
        merchTypes.forEach((d)=>{

          let detail = data.filter(x=>x.merchType == d && x.dataType == 'Capacity');
  
          let sumQty: number = 0.00;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          let sData = {merchType: d,
            dataType: "Capacity",
            details: detail,
            totalQty: sumQty,
            uom: displayUom,
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimePendingResult.push(sData);  

          detail = data.filter(x=>x.merchType == d && x.dataType == 'Booking');
          sumQty = 0;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          sData = {merchType: d,
            dataType: "Booking",
            details: detail,
            totalQty: sumQty,
            uom: displayUom,
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimePendingResult.push(sData);      
  
          detail = data.filter(x=>x.merchType == d && x.dataType == 'Truck');
  
          sumQty = 0;
  
          detail.forEach(item => {
            sumQty += item.quantity;
          });
  
          sData = {merchType: d,
            dataType: "Truck",
            details: detail,
            totalQty: sumQty,
            uom: "Truck",
            cap: this.operations.filter(x=>x.operationName == d)[0].capacity,
          };
  
          this.slottimePendingResult.push(sData);  
        })
  
        // summary truck per hour
        this.summaryPendingData = [];
  
        for (let index = 0; index < this.timeSlot; index++) {
  
          let cSlot: Date;
          let cSum: number;
          
          // cSlot = this.slottimeResult[index].slottime;
          cSum = 0;
          
          merchTypes.forEach((d)=>{          
            let re = this.slottimePendingResult.filter(x=>x.dataType == 'Truck' && x.merchType == d);
            cSum += re[0].details[index].quantity;
          });
  
          this.summaryPendingData.push({slottime: index,quantity: cSum})
        }
  
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
  
  onWarehouseSelected(event: any): void {
    this.companySelected = null;
    this.companies = [];
    this.warehouses.filter(x=>x.warehouseWms == event.value).forEach(whse => {
      this.companies.push({companyCode: whse.companyCode});
    });
  }
}
