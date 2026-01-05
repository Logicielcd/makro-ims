import { Location } from '@angular/common';
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
import { SupplierService } from '@core/services/master/supplier.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';

/* model */
import { BookingDetail, BookingHeaderDto, BookingTruckCheckIn, BookingTruckCheckInDetail, PreCheckIn } from '@core/models/booking/booking-header.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Supplier } from '@core/models/master/supplier.model';
import { User } from '@core/models/account/user.model';
import { TruckMaster } from '@core/models/master/truck-master.model';

import { ActivatedRoute, Router } from '@angular/router';
import { PoDialogComponent } from './po-dialog/po-dialog.component';
import { PagingService } from '@core/services/paging.service';

@UntilDestroy()
@Component({
  templateUrl: './pre-check-in-completed-detail.component.html',  
})
export class PreCheckInCompletedDetailComponent implements OnInit {

  title = 'Truck Pending';

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

  totalTruck:string;

  cols: any[];

  @ViewChild('f') f: NgForm;
  @ViewChild("poCheckInFocus") _pocheckinfocus: ElementRef;
  
  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    public route: ActivatedRoute,
    private warehouseService: WarehouseService,
    private bookingHeaderService: BookingHeaderService,    
    private authService: AuthService,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private supplierService: SupplierService,
    private truckMasterService: TruckMasterService,
    private _location: Location,
    private router: Router,
    private pagingService: PagingService,
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
            next: (data2) =>
            {              
              this.bookingHeader = data2;
              this.bookingHeader.bookingStart = new Date(this.bookingHeader.bookingStart);
              this.bookingHeader.bookingEnd = new Date(this.bookingHeader.bookingEnd);

              let truckRunning = 1;
              data2.bookingCheckIns.forEach(chkIn => {            
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
              this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});          
              });
            }
          }
          );
      }, error: (error) =>
      {
        this.msgs = [];
        error.Messages.forEach((msg: any) => {                    
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});          
        });
      }
    });   
  }

  checkPreCheckInComplete(){    
    // check all po already pre-checkin
    let totalPoBooking = this.bookingHeader.bookingDetails.length;
    let totalPoPrecheckin = 0;
    let poLists: string[] = [] as string[];

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

    if(totalPoBooking == totalPoPrecheckin){
      this.isCompleted = true;
    }
    else{
      this.isCompleted = false;
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
      this.bookingHeader.bookingCheckIns.push(truckCheckIn);
    },100); 

  }

  getPo(internalTruckCheckInId:number,element:any) {    
    this.ref = this.dialogService.open(PoDialogComponent, {
        header: 'Choose a PO',
        width: '1280px',
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
              this.messageService.add({severity:'error', summary: 'Error', detail: 'Duplicate PO', life: 3000});          
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
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Not found PO in this booking', life: 3000});
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
          this.messageService.add({severity:'error', summary: 'Error', detail: 'Duplicate PO', life: 3000});          
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
    let preCheckIn = {} as PreCheckIn;
    preCheckIn.bookingTruckCheckIns = this.bookingHeader.bookingCheckIns;
    preCheckIn.internalHeaderKey = this.bookingHeader.internalHeaderKey;
    preCheckIn.warehouseCode = this.bookingHeader.warehouseCode;
    preCheckIn.userStamp = this.user.userId;
    
    preCheckIn.bookingTruckCheckIns.forEach(chkIn => {
      chkIn.poNo = '';
      chkIn.userStamp = this.user.userId;
    });

    this.bookingHeaderService.savePreCheckIn(preCheckIn)
    .pipe(untilDestroyed(this))
    .subscribe(
      {next: (data) =>
        {           
          this.messageService.add({severity:'success', summary: 'Successful', detail: 'Pre-Check In success', life: 3000});

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
        this.messageService.add({severity:'success', summary: 'Successful', detail: 'PO Deleted', life: 3000});

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
        this.messageService.add({severity:'success', summary: 'Successful', detail: 'Delete truck success.', life: 3000});

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

  deletePreCheckIn(){
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this pre-check in?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.bookingHeaderService
        .deletePreCheckIn(this.bookingHeader.internalHeaderKey)
        .pipe(untilDestroyed(this))
        .subscribe((result) => { 
          this.messageService.add({severity:'success', summary: 'Delete Pre-check in completed', detail: ""});     
          this._location.back();
        });
      }
    });   
  }

  onBack() {
    let paths = this.router.url.split('/');    
    let path = '';
    for (let index = 0; index < paths.length-1; index++) {
      path += paths[index] + '/'
    }
    this.pagingService.isBack = true;
    this.router.navigate([path], {
      queryParams: {
        page: this.pagingService.curPage,
        pageSize: this.pagingService.pageSize,
      },
    });
  }

  

}