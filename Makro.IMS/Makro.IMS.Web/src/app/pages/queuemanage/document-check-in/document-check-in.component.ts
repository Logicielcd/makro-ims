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
import { BookingKeyService } from '@core/services/booking/booking-key.service';
import { GuardCheckInOutService } from '@core/services/booking/check-in.service';

/* model */
import { BookingTruck, BookingTruckCheckIn,BookingCheckOut, BookingCheckInDto,CheckIn } from '@core/models/booking/booking-header.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Supplier } from '@core/models/master/supplier.model';
import { PoList } from '@core/models/booking/po.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { User } from '@core/models/account/user.model';
import { TruckMaster } from '@core/models/master/truck-master.model';

@UntilDestroy()
@Component({
  templateUrl: './document-check-in.component.html',
})
export class DocumentCheckInComponent implements OnInit {

  title = 'Document Check-In';

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
  bookingCheckIns: BookingCheckInDto[];

  isCompleted: boolean = false;
  byPo: boolean = true;
  inputLabel: string;

  // po variable  
  poNo:string;
  poNoCheckout:string;

  totalTruck:string;

  cols: any[];
  isSelected:boolean;

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
    private guardCheckInOutService: GuardCheckInOutService,
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

      this.user = this.authService.getUser();
  }

  ngAfterViewInit(){
    // this._pofocus.nativeElement.focus();
  }

  ngOnInit() {
    this.cols = [
      {
        field: 'warehouseCode',
        display: 'string',        
        header: 'Warehouse Code',
      },
      {
        field: 'merchType',
        display: 'string',        
        header: 'Operation Type',
      },
      {
        field: 'bookingId',
        display: 'string',
        header: 'Booking Id',
      },
      {
        field: 'poNbr',
        display: 'string',        
        header: 'PoNo',
      },
      {
        field: 'bookingDate',
        display: 'date',        
        header: 'Booking Date',
      },
      {
        field: 'truckType',
        display: 'string',        
        header: 'Truck Type',
      },
      {
        field: 'driverName',
        display: 'string',        
        header: 'Driver name',
      },
      {
        field: 'licensePlate',
        display: 'string',        
        header: 'License Plate',
      },
      {
        field: 'telNo',
        display: 'string',        
        header: 'Tel No.',
      },
      {
        field: 'lineId',
        display: 'string',        
        header: 'Line Id',
      },
    ];

    this.isSelected = false;
    this.user = this.authService.getUser();
    this.inputLabel = "PO Nbr."

    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe((data)=>{
      this.warehouses = data;
      if(this.user.warehouseCode !== undefined || this.user.warehouseCode !== null){
        this.selectedWhse = this.user.warehouseCode;        
      }
    })

    this.supplierService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => { 
        this.suppliers = data;      
        this.getSupplierValue();
      });

    /*
    if(this.user.userType.toUpperCase() === "DHL")
    {
      this.supplierService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => { 
        this.suppliers = data;      
        this.getSupplierValue();
      });
    }
    else{
      this.supplierService
      .getBySupGroup(this.user.internalSupGroupId)
      .pipe(untilDestroyed(this))
      .subscribe((data) => { 
        this.suppliers = data;      
        this.getSupplierValue();
      });  
    }
    */
  }

  onWhseChange(){

  }

  onSelectSup(){    
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);
    this.supplierSelected = sup.supCode;    

    // get booking pending by supCode
    // this.bookingHeaderService.getPoCheckIn(this.supplierSelected,'123')
    this.guardCheckInOutService.getPoCheckIn(this.supplierSelected,'1')
    .pipe(untilDestroyed(this))
    .subscribe((data)=>{      
      this.bookingCheckIns = data;
      this.isSelected = true;      
    });

  }

  getSupplierValue(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);    
  }

  checkPo(){    

    let checkIn= {} as CheckIn;
    checkIn.poNbr = this.poNo;
    checkIn.userStamp = this.user.userId;
    checkIn.supCode = this.supplierSelected;

    if(this.byPo){
      this.guardCheckInOutService.documentCheckInPo(checkIn)
        .pipe(untilDestroyed(this))
        .subscribe({
          next: (data) =>
          {
            this.bookingCheckIns = data;
            this.messageService.add({severity:'success', summary: 'Successful', detail: 'Check in success.', life: 3000});
          }, error: (error) =>
          {
            this.msgs = [];
            error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
          }
        });
    }
    else{
      this.guardCheckInOutService.documentCheckInBooking(checkIn)
        .pipe(untilDestroyed(this))
        .subscribe({
          next: (data) =>
          {
            this.bookingCheckIns = data.filter(x=>x.warehouseCode == this.selectedWhse);
            this.messageService.add({severity:'success', summary: 'Successful', detail: 'Check in success.', life: 3000});
          }, error: (error) =>
          {
            this.msgs = [];
            error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
          }
        });
    }
    this.poNo = '';
  }

  refresh(): void {
    window.location.reload();
  }

  cbOnChange(){    
    if(!this.byPo){
      setTimeout(() => {
        this.inputLabel = "Booking Id";
      }, 500)
    }
    else{
      setTimeout(() => {
        this.inputLabel = "PO Nbr";
      }, 500)      
    }
  }

  
}