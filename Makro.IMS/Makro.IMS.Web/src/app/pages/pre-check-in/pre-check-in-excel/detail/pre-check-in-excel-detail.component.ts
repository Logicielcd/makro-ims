import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { MenuItem, Message, MessageService,ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Location } from '@angular/common';


/* service */
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { SupplierService } from '@core/services/master/supplier.service';
import { PoService } from '@core/services/booking/po.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { BookingKeyService } from '@core/services/booking/booking-key.service';

/* model */
import { BookingKey,BookingHeader,BookingDetail, BookingTruck,BookingKeyDto,BookingHeaderDto, BookingTruckCheckIn, BookingTruckCheckInDetail, PreCheckIn } from '@core/models/booking/booking-header.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Supplier } from '@core/models/master/supplier.model';
import { User } from '@core/models/account/user.model';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { EstTime } from '@core/models/master/est-time.model';
import { Door } from '@core/models/master/door.model';
import { UserService } from '@core/services/account/user.service';
import { ThisReceiver } from '@angular/compiler';
import { ActivatedRoute, Router } from '@angular/router';
import { PoDialogComponent } from './po-dialog/po-dialog.component';
import { PagingService } from '@core/services/paging.service';
import { matches } from '@syncfusion/ej2-base';

@UntilDestroy()
@Component({
  templateUrl: './pre-check-in-excel-detail.component.html',  
})
export class PreCheckInExcelDetailComponent implements OnInit {

  title = 'Pre Check-In';

  user: User;
  
  items: MenuItem[];
  msgs: Message[] = [];

  dataId: number;

  ref: DynamicDialogRef;

  tabIndex: number = 0;

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  // auth
  supReadonly: boolean = false;

  // warehouse variable
  warehouse: Warehouse[];
  warehouseCapacity: WarehouseCapacity[];

  // supplier variable
  suppliers: Supplier[];
  supplierSelected: string;
  truckSelected: number;
  
  // booking data variable
  bookingHeader = {} as BookingHeaderDto;
  booingDetails = [] as BookingDetail[];
  bookingCheckIn = {} as BookingTruckCheckIn;
  bookingCheckIns = [] as BookingTruckCheckIn[];

  isCompleted: boolean = false;
  byPo: boolean = true;

  inputLabel: string;

  // po variable  
  poNo:string;

  truckTypes: TruckMaster[];

  cols: any[];

  truckBooking: any[];

  @ViewChild('f') f: NgForm;
  @ViewChild("poCheckInFocus") _pocheckinfocus: ElementRef;
  
  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    public route: ActivatedRoute,
    private warehouseService: WarehouseService,
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
    private _location: Location,
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

      this.user = this.authService.getUser();
  }

  ngOnInit() {    
    this.inputLabel = "PO Nbr";
    this.cols = [
      {
        field: 'poNbr',
        display: 'string',        
        header: 'PoNo',
      },            
    ];

    this.user = this.authService.getUser();

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

    const decodedId = atob(this.route.snapshot.params.internalHeaderKey);
    this.dataId = +decodedId;
    
    if (this.dataId) {
      this.fetchData();
    }

  }

  onSelectSup(){    
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);
    this.supplierSelected = sup.supCode;    
  }

  getSupplierValue(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);    
  }

  refresh(): void {
    window.location.reload();
  }
  
  setFocus() { 
    this._pocheckinfocus.nativeElement.focus(); 
  } 

  fetchData(){
    this.bookingHeaderService.getById(this.dataId)
    .pipe(untilDestroyed(this))
    .subscribe({
      next : (data) =>
      {
        this.bookingHeaderService.getByBookingId(data.bookingId,data.supCode)
        .pipe(untilDestroyed(this))
        .subscribe(
        {
          next: (data) =>
          {            
            this.bookingHeader = data;
            this.bookingHeader.bookingStart = new Date(this.bookingHeader.bookingStart);
            this.bookingHeader.bookingEnd = new Date(this.bookingHeader.bookingEnd);

            let truckRunning = 1;
            data.bookingCheckIns.forEach(chkIn => {            
              chkIn.displayText = "Truck No. : " + truckRunning.toString() + " - [" + this.truckTypes.find(x=>x.internalTruckId == chkIn.internalTruckId).truckName + "]";
              truckRunning++;
            });
            
            this.checkPreCheckInComplete();

            let truckGroups: any[] = [];
            this.bookingHeader.bookingTrucks.forEach( t =>{
              let tr = this.truckTypes.filter(x=>x.internalTruckId == t.internalTruckId);
              truckGroups.push(tr[0].truckGroup);
            });

            let truckMasters: TruckMaster[] = [];
            truckMasters = this.truckTypes;
            
            truckGroups = Array.from(new Set(truckGroups));

            this.truckTypes = [];
            truckGroups.forEach( d => {
              this.truckTypes.push(...truckMasters.filter(x=>x.truckGroup == d));
            })
            
          }, error: (error) => 
          {
            this.msgs = [];
            error.Messages.forEach((msg: any) => {                    
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});          
            });
          }
        }
      );
      }, error: (error) =>
      {
        this.msgs = [];
        error.Messages.forEach((msg: any) => {                    
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});          
        });
      }
    });    
  }

  checkPreCheckInComplete():boolean{
    if(this.bookingHeader.bookingDetails !== null && this.bookingHeader.bookingDetails !== undefined){
      // check all po already pre-checkin
      let totalPoBooking = this.bookingHeader.bookingDetails.length;
      let totalPoPrecheckin = 0;
      let poLists: string[] = [] as string[];
      let isError = false;

      this.bookingHeader.bookingCheckIns.forEach(booking => {
        if(booking.driverName === undefined || booking.driverName === null || booking.driverName.length === 0 || 
          booking.licensePlate === undefined || booking.licensePlate === null || booking.licensePlate.length === 0 || 
          booking.telNo === undefined || booking.telNo === null || booking.telNo.length === 0 || 
          booking.lineId === undefined || booking.lineId === null || booking.lineId.length === 0){            
            isError = true;
        };        
      });

      const pattern = /^(?:\d{1}[ก-ฮ]{1,2}|\d{3}|\d{2}|[ก-ฮ]{1,2}?)[-]{1}\d{1,4}$/;     
      // validate truck license plate
      this.bookingHeader.bookingCheckIns.forEach(booking =>{
        if(booking.licensePlate !== undefined)
        {
          const matches = booking.licensePlate.match(pattern);
          if(matches === null){
            this.isCompleted = false;
            isError = true;
          }
          else{
            if(matches[0] !== booking.licensePlate){
              this.isCompleted = false;
              isError = true;
            }
          }
        }
        if(booking.licensePlate2 !== undefined && booking.licensePlate2 !== null && booking.licensePlate2.length > 0) 
        {
          const matches = booking.licensePlate2.match(pattern);
          if(matches === null){
            
            this.isCompleted = false;
            isError = true;
          }
          else{
            if(matches[0] !== booking.licensePlate2){
              this.isCompleted = false;
              isError = true;
            }
          }
        }

        if(booking.telNo !== undefined && booking.telNo.length > 0)
        {
          const phonePattern = /^0\d{9}$/;
          const phoneMatch = booking.telNo.match(phonePattern);
          if(phoneMatch === null){
            isError = true;
          }
        }
      });

      this.bookingHeader.bookingCheckIns.forEach(chkIn => {
        chkIn.bookingTruckCheckInDetails.forEach(chkDtl => {
          poLists.push(chkDtl.poNbr);
        });
      });

      let poResult: string[] = [];
      poLists.forEach((item) => {
        if (!poResult.includes(item)) {
          poResult.push(item);
        }
      })

      totalPoPrecheckin = poResult.length;

      if(totalPoBooking === totalPoPrecheckin && this.bookingHeader.status.toLowerCase() === "approved" && isError === false){
        this.isCompleted = true;
        return true;
      }else{
        this.isCompleted = false;
        return false;
      }
    }
  }

  addTruck(){
    let truckCheckIn = {} as BookingTruckCheckIn;
    let truckRunning: number = 0;
    if (this.bookingHeader.bookingCheckIns ===null || this.bookingHeader.bookingCheckIns === undefined){
      truckRunning = 1;
    }
    else{
      truckRunning = this.bookingHeader.bookingCheckIns.length + 1;
    }

    truckCheckIn.bookingTruckCheckInDetails = [] as BookingTruckCheckInDetail [];
    truckCheckIn.internalTruckCheckInId = truckRunning;
    truckCheckIn.userStamp = this.user.userId;
    truckCheckIn.internalHeaderKey = this.bookingHeader.internalHeaderKey;
    truckCheckIn.internalTruckId = this.truckSelected;

    setTimeout(()=>{ // this will make the execution after the above boolean has changed
      truckCheckIn.displayText = "Truck No. : " + truckRunning.toString() + " - [" + this.truckTypes.find(x=>x.internalTruckId == this.truckSelected).truckName + "]";
      
      this.bookingHeader.bookingDetails.forEach( po => {

        // assign available PO to truck
        let checkDupPo = this.bookingHeader.bookingCheckIns.find(x=>x.bookingTruckCheckInDetails.find(x=>x.poNbr == po.poNbr));

        if(checkDupPo == null || checkDupPo == undefined)
        {
          let bkDtl: BookingTruckCheckInDetail = {} as BookingTruckCheckInDetail;
          bkDtl.internalTruckCheckInId = truckCheckIn.internalTruckCheckInId;
          bkDtl.poNbr = po.poNbr;
          truckCheckIn.bookingTruckCheckInDetails.push(bkDtl);
        }
        
      });

      this.bookingHeader.bookingCheckIns.push(truckCheckIn);
      
    },100); 

    this.checkPreCheckInComplete();
  }

  getPo(internalTruckCheckInId:number,element:any) {
    
    this.ref = this.dialogService.open(PoDialogComponent, {
        header: 'Choose a PO',
        width: '750px',
        contentStyle: {"max-height": "1000px", "overflow": "auto"},
        baseZIndex: 10000,
        data: {
          poList: this.bookingHeader.bookingDetails,          
        }
    });

    this.ref.onClose.subscribe((ret: any) =>{
      if (ret)
      {        
        ret.forEach(po => {

          let checkIn = this.bookingHeader.bookingCheckIns.find(x=>x.internalTruckCheckInId == internalTruckCheckInId);

          let chkInDtl = {} as BookingTruckCheckInDetail;
          chkInDtl.internalTruckCheckInId = internalTruckCheckInId;
          chkInDtl.poNbr = po.poNbr;

          if(checkIn.bookingTruckCheckInDetails === undefined){
            checkIn.bookingTruckCheckInDetails = [] as BookingTruckCheckInDetail[];
          }
          else{
            // check duplicate po
            let chkDupPo = checkIn.bookingTruckCheckInDetails.find(x=>x.poNbr == po.poNbr);
            if(chkDupPo !== null && chkDupPo !== undefined){      
              this.messageService.add({severity:'error', summary: 'Error', detail: 'Duplicate PO', life: 10000});          
              return;
            }
          }

          this.bookingHeader.bookingCheckIns.find(x=>x.internalTruckCheckInId == internalTruckCheckInId).bookingTruckCheckInDetails.push(chkInDtl);
          checkIn.poNo = '';
        });

            
        this.checkPreCheckInComplete();
        
      }
    });
  }

  checkInPo(internalTruckCheckInId:number,element:any){
    this.bookingCheckIn = {} as BookingTruckCheckIn;

    let checkIn = this.bookingHeader.bookingCheckIns.find(x=>x.internalTruckCheckInId == internalTruckCheckInId);

    let checkPo = this.bookingHeader.bookingDetails.find(x=>x.poNbr == checkIn.poNo);

    if(checkPo === null || checkPo === undefined){
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Not found PO in this booking', life: 10000});
    }
    else{
      // create truck booking check in detail
      let chkInDtl = {} as BookingTruckCheckInDetail;
      chkInDtl.internalTruckCheckInId = internalTruckCheckInId;
      chkInDtl.poNbr = checkIn.poNo;

      if(checkIn.bookingTruckCheckInDetails === undefined){
        checkIn.bookingTruckCheckInDetails = [] as BookingTruckCheckInDetail[];
      }
      else{
        // check duplicate po
        let chkDupPo = checkIn.bookingTruckCheckInDetails.find(x=>x.poNbr == checkIn.poNo);
        if(chkDupPo !== null && chkDupPo !== undefined){      
          this.messageService.add({severity:'error', summary: 'Error', detail: 'Duplicate PO', life: 10000});          
          return;
        }
      }

      this.bookingHeader.bookingCheckIns.find(x=>x.internalTruckCheckInId == internalTruckCheckInId).bookingTruckCheckInDetails.push(chkInDtl);
    }

    this.checkPreCheckInComplete();
    checkIn.poNo = '';
  }

  savePreCheckIn()
  {
    this.isLoading = true;
    let preCheckIn = {} as PreCheckIn;

    preCheckIn.bookingTruckCheckIns = this.bookingHeader.bookingCheckIns;
    preCheckIn.internalHeaderKey = this.bookingHeader.internalHeaderKey;
    preCheckIn.warehouseCode = this.bookingHeader.warehouseCode;
    preCheckIn.userStamp = this.user.userId;
    
    preCheckIn.bookingTruckCheckIns.forEach(chkIn => {
      chkIn.poNo = '';
      chkIn.userStamp = this.user.userId;
      chkIn.licensePlate2 = chkIn.licensePlate2 == null ? '':chkIn.licensePlate2;
    });

    this.bookingHeaderService.savePreCheckIn(preCheckIn)
    .pipe(untilDestroyed(this))
    .subscribe(
      {next: (data) =>
        {           
          this.messageService.add({severity:'success', summary: 'Successful', detail: 'Pre-Check In success', life: 10000});
          this.isLoading = false;
          setTimeout(()=>{
            this.onBack();
          },1000)
          
        }, error: (error) => 
        {
          this.msgs = [];
          error.Messages.forEach((msg: any) => {                    
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});          
          });
          this.isLoading = false;
        }
      }
    ); 

  }

  trackByMethod(index: number, el: any): number {
    return el.id;
  }

  tabChange(e:any){
    this.tabIndex = e.index;
    setTimeout(()=>{ 
      this._pocheckinfocus.nativeElement.focus();
    },100); 
  }

  deletePo(delPo:any){    
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected PO?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.bookingHeader.bookingCheckIns.find(x=>x.internalTruckCheckInId === delPo.internalTruckCheckInId).bookingTruckCheckInDetails = 
        this.bookingHeader.bookingCheckIns.find(x=>x.internalTruckCheckInId === delPo.internalTruckCheckInId).bookingTruckCheckInDetails.filter(x=>x.poNbr !== delPo.poNbr);
        this.messageService.add({severity:'success', summary: 'Successful', detail: 'PO Deleted', life: 10000});

        this.checkPreCheckInComplete();
      }
    });   
  }

  deleteTruck(delTruck:any){    
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected Truck?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        
        this.bookingHeader.bookingCheckIns = this.bookingHeader.bookingCheckIns.filter(x=>x.displayText !== delTruck.displayText);
        this.messageService.add({severity:'success', summary: 'Successful', detail: 'Delete truck success.', life: 10000});

        // re-running truck
        let running = 1;
        this.bookingHeader.bookingCheckIns.forEach(truck => {
          setTimeout(()=>{ // this will make the execution after the above boolean has changed
            truck.internalTruckCheckInId = running;
            truck.displayText = "Truck No. : " + running.toString() + " - [" + this.truckTypes.find(x=>x.internalTruckId == truck.internalTruckId).truckName + "]";            
            running++;
          },100); 
          
        });

        this.checkPreCheckInComplete();
      }
    });   
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

  onBack(){
    this._location.back();
  }

  validateAndFormatPhoneNumber(event: Event): void {
    let input = (event.target as HTMLInputElement).value;

    // Remove all non-numeric characters
    input = input.replace(/\D/g, '');

    // Ensure the first digit is 0
    if (input.length > 0 && input[0] !== '0') {
      input = '0' + input.substring(0, 9); // Add leading 0 and truncate to 10 digits
    }

    // Limit to 10 digits
    if (input.length > 10) {
      input = input.substring(0, 10);
    }

  }

}