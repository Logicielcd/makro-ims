import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, MenuItem, Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';

/* service */
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { BookingKeyService } from '@core/services/booking/booking-key.service';
import { PoService } from '@core/services/booking/po.service';
import { DoorService } from '@core/services/master/door.service';
import { EstTimeService } from '@core/services/master/est-time.service';
import { SupplierService } from '@core/services/master/supplier.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { WarehouseCapacityService } from '@core/services/master/warehouse-capacity.service';
import { WarehouseService } from '@core/services/master/warehouse.service';

/* model */
import { User } from '@core/models/account/user.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { BookingDetail, BookingHeader, BookingHeaderDto, BookingKey, BookingKeyDto, BookingTruck } from '@core/models/booking/booking-header.model';
import { PoList } from '@core/models/booking/po.model';
import { Door } from '@core/models/master/door.model';
import { EstTime } from '@core/models/master/est-time.model';
import { Supplier } from '@core/models/master/supplier.model';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Warehouse } from '@core/models/master/warehouse.model';

import { environment } from '@environments/environment';
import { Operation } from '@core/models/master/operation.model';
import { OperationService } from '@core/services/master/operation.service';
import { ManualBookingDto } from '@core/models/booking/check-in-out.model';
import { GuardCheckInOutService } from '@core/services/booking/check-in.service';

@UntilDestroy()
@Component({
  templateUrl: './guard-create-booking.component.html',
  styleUrls: ['guard-create-booking.component.scss'],
})
export class GuardCreateBookingComponent implements OnInit {

  title = 'Manual Create Booking';

  user: User;

  items: MenuItem[];
  msgs: Message[] = [];

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  isSelect: boolean = false;

  ref: DynamicDialogRef;

  // auth
  supReadonly: boolean = false;

  // warehouse variable
  warehouse: Warehouse[];
  warehouseCapacity: WarehouseCapacity[];

  warehouses: Warehouse[];
  warehouseSelected: string;

  operations: Operation[] = [];
  operationSelected: string;

  // supplier variable
  suppliers: Supplier[];
  supplierSelected: string;

  // booking data variable
  bookingDate: Date;

  // truck variable
  trucks: TruckMaster[];
  truckSelected: string;

  preLicense: string;
  licensePlate: string;
  preLicense2: string;
  licensePlate2: string;
  lineNo: string;
  telNo: string;
  driverName: string;

  contactName: string;
  contactPhone: string;
  contactEmail: string;

  minDate: Date;
  msgBooking: string;

  data: ManualBookingDto;

  @ViewChild('f') f: NgForm;

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    private warehouseService: WarehouseService,
    private warehouseCapacityService: WarehouseCapacityService,    
    private authService: AuthService,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private supplierService: SupplierService,
    private truckMasterService: TruckMasterService,    
    private operationService: OperationService,
    private checkinoutService: GuardCheckInOutService
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

    this.bookingDate = new Date();
    // this.bookingDate.setDate(this.bookingDate.getDate() + 1);
    this.user = this.authService.getUser();

    this.minDate = new Date();
    // this.minDate.setDate(this.minDate.getDate() + 1);

  }

  ngOnInit() {
    this.isSelect = false;
    this.data = null;
    this.operationSelected = null;
    this.supplierSelected = null;
    this.truckSelected = null;
    this.preLicense = null;
    this.licensePlate = null;
    this.preLicense2 = null;
    this.licensePlate2 = null;
    this.driverName = null;
    this.telNo = null;
    this.lineNo = null;
    this.fetchData();

  }

  initialScreen(){
    this.isSelect = false;
    this.fetchData();
  }

  onCreateTrip() {
    this.confirmDialog = true;
  }

  fetchData() {
    this.warehouseService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.warehouse = data;
        this.warehouses = data;
        this.warehouse.forEach(x=>x.warehouseDisplay = x.warehouseCode + '-' + x.warehouseName);
      },(error)=>{
        this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});
          });
      });


    if(this.user.userType.toUpperCase() != "SUP"){
      this.supplierService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.suppliers = data;
        this.getSupplierValue();
      },(error)=>{
        this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});
          });
      });
    }
    else{

    this.supplierService
      .getBySupGroup(this.user.internalSupGroupId)
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.suppliers = data;
        this.getSupplierValue();
      },(error)=>{
        this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});
          });
      });
    }

    this.truckMasterService
    .getAll()
    .pipe(untilDestroyed(this))
    .subscribe((data) => {
      this.trucks = data;
    },(error)=>{
      this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});
          });
    });

    this.warehouseCapacityService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.warehouseCapacity = data;
      },(error)=>{
        this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});
          });
      });
  }

  onWarehouseChange(event:any){
    this.warehouseSelected = event.value;    
    this.operationService.getByWarehouse(this.warehouseSelected)
    .pipe(untilDestroyed(this))
    .subscribe((d)=>{             
      this.operations = d;             
    });
  }

  isCompleted():boolean{    
    if( this.isNull(this.warehouseSelected) ||      
        this.isNull(this.operationSelected) || 
        this.isNull(this.supplierSelected) || 
        this.isNull(this.truckSelected) || 
        this.isNull(this.driverName) || 
        this.isNull(this.telNo) || 
        this.isNull(this.lineNo) || 
        this.isNull(this.contactName) ||
        this.isNull(this.contactPhone) || 
        this.isNull(this.contactEmail)
      ){
        return true;
      }
      else{
        return false;
      }
  }

  getSupplierValue(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);
    if(sup != null && sup != undefined){
      this.contactName = sup.contactName;
      this.contactEmail = sup.contactEMail;
      this.contactPhone = sup.phoneNumber;
    }
  }

  onSaveBooking():void{
    this.isLoading = true;

    let sup = this.suppliers.filter(x=>x.supCode == this.supplierSelected)[0];
    let timezone = 0;
    let diffTime = 0;

    this.data = {} as ManualBookingDto;

    timezone = Math.abs(new Date().getTimezoneOffset());

    if(timezone != 420){
      if(timezone > 420){
        diffTime = timezone - 420;
      }
      else{
        diffTime = 420 - timezone;  
      }      

      let bookTime = new Date();      
      bookTime.setMinutes(bookTime.getMinutes() + diffTime);      
      this.data.bookingDate = bookTime;
    }
    else{
      this.data.bookingDate = new Date();
    }

    
    this.data.warehouseCode = this.warehouseSelected;
    this.data.operationType = this.operationSelected;
    this.data.supCode = sup.supCode;
    this.data.supName = sup.supName;
    this.data.subGroupId = sup.internalGroupId;
    this.data.truckType = this.truckSelected.toString();
    this.data.driverName = this.driverName;
    this.data.licensePlate = this.preLicense + '-' + this.licensePlate;
    this.data.licensePlate2 = this.preLicense2 !== undefined && this.preLicense2 !== null ? this.preLicense2 + '-' + this.licensePlate2 : ''; 
    this.data.telNo = this.telNo;
    this.data.lineNo = this.lineNo;
    this.data.userName = this.user.userId;
    this.data.contactEmail = this.contactEmail;
    this.data.contactName = this.contactName;
    this.data.contactPhone = this.contactPhone;

    this.checkinoutService
        .manualBooking(this.data)
        .pipe(untilDestroyed(this))
        .subscribe(
          {
            next:(result) => {
              this.ngOnInit();
              this.messageService.add({severity:'success', summary: 'Create Booking completed', detail: ""});
              this.msgs = [];
              result.result.forEach(msg => {
                this.msgs.push({
                  severity: 'success',
                  summary: 'Booking success',
                  detail: msg,
                });
              });
              this.isLoading = false;              
            },
            error: (error) =>{
              this.isLoading = false;
              this.msgs = [];
              error.Messages.forEach((msg: any) => {
                this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});
              });
              this.isLoading = false;
            }
          }
        );
  }

  refresh(): void {
    window.location.reload();
  }

  keyPressNumbers(event) {
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57)) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
  }

  isNull(value: string): boolean{
    if(value === null || value === undefined){      
      return true;
    }
    else{
      if(value === ''){
        return true;
      }
      else{
        return false;
      }
    }
  }

}
