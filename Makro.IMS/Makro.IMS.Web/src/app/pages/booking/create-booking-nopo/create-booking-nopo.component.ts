import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, MenuItem, Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';

/* service */
import { EstTimeService } from '@core/services/master/est-time.service';
import { SupplierService } from '@core/services/master/supplier.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { WarehouseCapacityService } from '@core/services/master/warehouse-capacity.service';
import { WarehouseService } from '@core/services/master/warehouse.service';

/* model */
import { User } from '@core/models/account/user.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { Door } from '@core/models/master/door.model';
import { EstTime } from '@core/models/master/est-time.model';
import { Supplier } from '@core/models/master/supplier.model';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Warehouse } from '@core/models/master/warehouse.model';

/* component */
import { SlotTimeModalComponent } from '@theme/components/modal/slot-time-modal/slot-time-modal.component';

import { Operation } from '@core/models/master/operation.model';
import { OperationService } from '@core/services/master/operation.service';
import { ManualBookingDto } from '@core/models/booking/check-in-out.model';
import { InboundBookingService } from '@core/services/booking/inbound-booking.service';
import { InboundBookingDto } from '@core/models/booking/inbound-booking';

@UntilDestroy()
@Component({
  templateUrl: './create-booking-nopo.component.html',
  styleUrls: ['create-booking-nopo.component.scss'],
})
export class CreateBookingNopoComponent implements OnInit {

  title = 'Create Inbound Booking';

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

  startTime: Date;
  endTime: Date;

  data: InboundBookingDto;

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
    private inboundBookingService: InboundBookingService
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
    this.startTime = null;
    this.endTime = null;
    

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
        
        if(this.user.warehouseCode !== null && this.user.warehouseCode !== undefined){
          this.warehouseSelected = this.user.warehouseCode;
          this.operationService.getByWarehouse(this.warehouseSelected)
          .pipe(untilDestroyed(this))
          .subscribe((d)=>{             
            this.operations = d;            
            if(this.user.operationType !== null && this.user.operationType !== undefined && this.operations.find(x=>x.operationName == this.user.operationType)){
              this.operationSelected = this.user.operationType;            
            }
          });
        }

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

      if(this.user.operationType !== null && this.user.operationType !== undefined && this.operations.find(x=>x.operationName == this.user.operationType)){
        this.operationSelected = this.user.operationType;
      }
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
        this.isNull(this.contactEmail) ||
        this.isNull(this.startTime) ||
        this.isNull(this.endTime) 
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

    this.data = {} as InboundBookingDto;

    timezone = Math.abs(new Date().getTimezoneOffset());

    if(timezone != 420){
      if(timezone > 420){
        diffTime = timezone - 420;
      }
      else{
        diffTime = 420 - timezone;  
      }      
      let bookTime = new Date();      
      let bookingStartTime = new Date();
      let bookingEndTime = new Date();
      bookTime.setMinutes(bookTime.getMinutes() + diffTime);
      bookingStartTime = this.startTime;
      bookingStartTime.setMinutes(bookingStartTime.getMinutes() + diffTime);
      bookingEndTime = this.endTime;
      bookingEndTime.setMinutes(bookingEndTime.getMinutes() + diffTime);

      this.data.bookingDate = bookTime;
      this.data.startTime = bookingStartTime;
      this.data.endTime = bookingEndTime;
    }
    else{
      this.data.bookingDate = new Date();
      this.data.startTime = this.startTime;
      this.data.endTime = this.endTime;
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

    this.inboundBookingService
        .inboundBooking(this.data)
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

  isNull(value: any): boolean{
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

  isShow():boolean{    
    if( this.isNull(this.warehouseSelected) ||      
        this.isNull(this.operationSelected) || 
        this.isNull(this.supplierSelected) || 
        this.isNull(this.truckSelected)
      ){
        return true;
      }
      else{
        return false;
      }
  }

  show() 
  {

    let bookingCreate = {} as BookingCreate;
    bookingCreate.bookingDate = this.bookingDate;
    bookingCreate.merchType = this.operationSelected;
    bookingCreate.companyCode = this.warehouse.find(x=>x.warehouseCode == this.warehouseSelected).companyCode;
    bookingCreate.warehouseCode = this.warehouseSelected;
    bookingCreate.nextShift = new Date();

    const modalComponent = SlotTimeModalComponent;

    this.ref = this.dialogService.open(modalComponent, {
      header: 'Choose a time slot',
      width: '1280px',
      contentStyle: {"max-height": "1000px", "overflow": "auto"},
      baseZIndex: 10000,
      data: {
        warehouseCode: this.warehouseSelected,
        bookingDate: this.bookingDate,
        supplierCode: this.supplierSelected,
        supplierName: this.suppliers.find(x=>x.supCode == this.supplierSelected).supName,
        totalEstTime: 60,
        bookingSlot: bookingCreate,
        internalSupGroupId: this.suppliers.find(x=>x.supCode == this.supplierSelected).internalGroupId,
        totalWeight: 0,
      }
    });

    var door: Door = {} as Door;
    var doorId: number = 0;
    var doorName: string = '';    
    var returnData: any;

    this.ref.onClose.subscribe((ret:any)=>{
      if (ret){                
                
          doorId = 0;
          doorName = '';
          returnData = ret;
          //console.log(returnData);
          this.startTime = returnData.StartTime;
          this.endTime = returnData.EndTime;
          
      }
    });

  }
}
