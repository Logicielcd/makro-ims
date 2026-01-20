import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { MenuItem, Message, MessageService,ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';


/* service */
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { WarehouseCapacityService } from '@core/services/master/warehouse-capacity.service';
import { SupplierService } from '@core/services/master/supplier.service';
import { PoService } from '@core/services/booking/po.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';


/* model */
import { BookingKey,BookingHeader,BookingDetail, BookingTruck,BookingKeyDto,BookingHeaderDto, BookingTruckCheckIn,BookingCheckOut } from '@core/models/booking/booking-header.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Supplier } from '@core/models/master/supplier.model';
import { PoList } from '@core/models/booking/po.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { User } from '@core/models/account/user.model';
import { TruckMaster } from '@core/models/master/truck-master.model';

@UntilDestroy()
@Component({
  templateUrl: './document-confirmation.component.html',
})
export class DocumentConfirmationComponent implements OnInit {

  title = 'Check-In';

  user: User;
  
  items: MenuItem[];
  msgs: Message[] = [];

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  // auth
  supReadonly: boolean = false;

  // warehouse variable
  warehouses: Warehouse[];
  selectedWhse: string;
  warehouseCapacity: WarehouseCapacity[];

  // supplier variable
  suppliers: Supplier[];
  supplierSelected: string;
  
  // booking data variable
  bookingHeader = {} as BookingHeaderDto;
  booingDetails = [] as BookingDetail[];
  bookingCheckIn = {} as BookingTruckCheckIn;
  bookingCheckIns = [] as BookingTruckCheckIn[];
  bookingCheckOut = {} as BookingCheckOut;

  isCompleted: boolean = false;

  // po variable  
  poNo:string;
  poNoCheckout:string;

  truckTypes: TruckMaster[];

  totalTruck:string;

  totalPo: number;
  
  cols: any[];

  @ViewChild('f') f: NgForm;

  @ViewChild("poFocus") _pofocus: ElementRef;  
  @ViewChild("poCheckoutFocus") _pocheckoutfocus: ElementRef;
  

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    private warehouseService: WarehouseService,
    private warehouseCapacityService: WarehouseCapacityService,
    private bookingHeaderService: BookingHeaderService,    
    private poService: PoService,
    private authService: AuthService,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private supplierService: SupplierService,
    private truckMasterService: TruckMasterService,    
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

  }

  ngAfterViewInit(){
    this._pofocus.nativeElement.focus();
  }

  ngOnInit() {    
    this.cols = [
      {
        field: 'poNbr',
        display: 'string',        
        header: 'PoNo',
      },            
      {
        field: 'status',
        display: 'string',        
        header: 'Status',
      },            
    ];

    this.user = this.authService.getUser();

    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe((data)=>{
      this.warehouses = data;
      if(this.user.warehouseCode !== undefined || this.user.warehouseCode !== null){
        this.selectedWhse = this.user.warehouseCode;        
      }
    })
    
    if(this.user.userType.toUpperCase() !== "SUP")
    {
      this.truckMasterService.getAll().pipe(untilDestroyed(this)).subscribe((data) => {this.truckTypes = data});
      this.supplierService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => { 
        this.suppliers = data;      
        this.getSupplierValue();
      });  
    }
    else{
      this.truckMasterService.getAll().pipe(untilDestroyed(this)).subscribe((data) => {this.truckTypes = data});
      this.supplierService
      .getBySupGroup(this.user.internalSupGroupId)
      .pipe(untilDestroyed(this))
      .subscribe((data) => { 
        this.suppliers = data;      
        this.getSupplierValue();
      });  
    }
    
    // this.truckMasterService.getAll().pipe(untilDestroyed(this)).subscribe((data) => {this.truckTypes = data});
    // this.supplierService
    // .getAll()
    // .pipe(untilDestroyed(this))
    // .subscribe((data) => { 
    //   this.suppliers = data;      
    //   this.getSupplierValue();
    // });

  }

  onSelectSup(){    
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);
    this.supplierSelected = sup.supCode;    
  }

  getSupplierValue(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);    
  }

  checkPo(){    

    this.bookingCheckOut.poNo = this.poNo;
    this.bookingCheckOut.supCode = this.supplierSelected;
    this.bookingCheckOut.userStamp = this.user.name;

    this.bookingHeaderService.saveCheckOutPo(this.bookingCheckOut)
    .pipe(untilDestroyed(this))
    .subscribe(
      {
        next: (data) =>
        {
          this.bookingHeader = data;
          
          this.bookingHeader.bookingTrucks.forEach((truck: BookingTruck) =>{
            truck.bookingCheckins = [];
            truck.bookingCheckins = this.bookingHeader.bookingCheckIns.filter(x=>x.internalTruckId == truck.internalTruckId && x.licensePlate == truck.licensePlate);
          });
          
          // this._pocheckoutfocus.nativeElement.focus();
          this.messageService.add({severity:'success', summary: 'Success', detail: 'Check out PO : ' + this.poNo + ' success', life: 3000});   

          this.poNo = '';


        }, error: (error) => 
        {
          this.msgs = [];
          error.Messages.forEach((msg: any) => {                    
          this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});          
          });
        }
      }
    );
  }

  checkOutPo(){
    
    this.bookingCheckIn = {} as BookingTruckCheckIn;

    // this.bookingCheckIn.PoNbr = this.poNoCheckout;
    this.bookingCheckIn.internalHeaderKey = this.bookingHeader.internalHeaderKey;
    this.bookingCheckIn.userStamp = this.user.name;

    this.bookingHeaderService.saveCheckOut(this.bookingCheckIn)
    .pipe(untilDestroyed(this))
    .subscribe(
      {next: (data) =>
        {
          this.bookingHeader = data;
          

        }, error: (error) => 
        {
          this.msgs = [];
          error.Messages.forEach((msg: any) => {                    
          this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
        });
        }
      }
    );
  }

  refresh(): void {
    window.location.reload();
  }
  
}