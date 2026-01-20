import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseService } from '@core/services/master/warehouse.service';

import { NgForm } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';
import { ReportService } from '@core/services/report/report.service';
import { ReportSummaryBooking } from '@core/models/report/report-summarybooking';
import { Subject, interval, takeUntil } from 'rxjs';

@UntilDestroy()

@Component({  
  templateUrl: 'summary-booking.component.html',
  styleUrls: ['summary-booking.component.scss'],
  encapsulation: ViewEncapsulation.None,
})

export class SummaryBookingComponent implements OnInit {
  private destroy$: Subject<void> = new Subject<void>();
  
  user : User;
  warehouses: Warehouse[];
  warehouseList: any[];
  companies: any[];
  warehouseSelected: string = '';
  companySelected: string = '';
  
  summaryBooking: ReportSummaryBooking = {} as ReportSummaryBooking;

  reportDate: Date;

  visible: boolean = false;

  autoRefresh: boolean;
  refreshIntervalTime: number;
  refreshTime: Date;

  @ViewChild('f') f: NgForm;

  constructor(private reportService: ReportService
    ,private warehouseService: WarehouseService    
    ,private authService: AuthService
    ) {      
      this.user = this.authService.getUser();
      this.autoRefresh = false;
      this.refreshIntervalTime = 60;
      this.refreshTime = new Date();
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
      this.visible = false;
      this.refreshTime = new Date();

      this.reportService.getSummaryBooking(this.warehouseSelected,this.companySelected)
      .pipe(untilDestroyed(this))
      .subscribe((d)=>{
      this.summaryBooking = d;
      this.summaryBooking.totalPending = this.summaryBooking.totalBooking - this.summaryBooking.totalCheckIn;
      });
  }

  public onWhseChange(event: any):void {
    this.getBookingData();
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
    this.getBookingData();
  }

  refreshData(){
    this.getBookingData();
  }

  ngOnDestroy() {    
    this.destroy$.next();
    this.destroy$.complete();
  }

  onWarehouseSelected(event: any): void {
    this.companySelected = null;
    this.companies = [];
    this.warehouses.filter(x=>x.warehouseWms == event.value).forEach(whse => {
      this.companies.push({companyCode: whse.companyCode});
    });
  }
}
