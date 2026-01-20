import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
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
import { EstTimeService } from '@core/services/master/est-time.service';
import { DoorService } from '@core/services/master/door.service';
import { BookingKeyService } from '@core/services/booking/booking-key.service';

/* model */
import { BookingKey,BookingHeader,BookingDetail, BookingTruck,BookingKeyDto,BookingHeaderDto } from '@core/models/booking/booking-header.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Supplier } from '@core/models/master/supplier.model';
import { PoList } from '@core/models/booking/po.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { SlotTimeDtlComponent } from './dialog/slot-time-dtl.component';
import { User } from '@core/models/account/user.model';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { EstTime } from '@core/models/master/est-time.model';
import { Door } from '@core/models/master/door.model';
import { ActivatedRoute } from '@angular/router';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { concat, concatMap, map, switchMap } from 'rxjs';


@UntilDestroy()
@Component({
  selector: 'app-booking-header-detail',
  templateUrl: './manage-queue-detail.component.html',
})
export class ManageQueueDetailComponent implements OnInit {

  title = 'Booking Detail'

  user: User;
  msgs: Message[] = [];

  dataId: number;

  items: MenuItem[];
  
  confirmDialog: boolean = false;
  isLoading: boolean = false;

  isSelect: boolean = false;

  ref: DynamicDialogRef;

  // auth
  supReadonly: boolean = false;

  // warehouse variable
  warehouse: Warehouse[];
  warehouseCapacity: WarehouseCapacity[];

  // supplier variable
  supplierSelected: string;
  supplierDisplay: string;
  internalSupGroupId: number;
  supplierName: string;
  
  // booking data variable
  data = {} as BookingHeader;
  selectedDetail: BookingDetail[] = [];
  bookingKey: BookingKey = {} as BookingKey;
  orderDetails: BookingDetail[];

  bookingCreates: BookingCreate[] = [];
  bookingDate: Date;

  contactName:string;
  contactEmail:string;
  contactPhone:string;

  isCompleted: boolean = false;

  // po variable  
  poDtl: PoList;
  inputPoNo: string;
  cols: any[];

  // truck variable
  bookingTruck: BookingTruck;
  trucks: TruckMaster[];
  truckSelected: number;
  truckDriver: string;
  truckLicense: string;
  truckCols: any[];

  // estTime
  estTimes: EstTime[];
  totalEstTime: number;

  bookingKeyDto: BookingKeyDto;

  allowToEdit: boolean;

  minDate: Date;
  maxDate: Date;

  @ViewChild('f') f: NgForm;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    Validation: 'Message.Validation',
    CreateSuccess: 'Message.Notification.Create',
    EditSuccess: 'Message.Notification.Update',
    DeleteSuccess: 'Message.Notification.Delete',
    DeleteTitle: 'Message.Confirm.Delete.Title',
    DeleteMessage: 'Message.Confirm.Delete.Message',
    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };
  
  canModify: boolean;

  constructor(
    public route: ActivatedRoute,
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
    private estTimeService: EstTimeService,
    private doorService: DoorService,
    private bookingKeyService: BookingKeyService,
    private _location: Location
  ) {

    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.warehousecapacity)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));

    this.canModify = true;

    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
    
    this.bookingDate = new Date();
    this.bookingDate.setDate(this.bookingDate.getDate() + 1);
    this.user = this.authService.getUser();

    let dateNow = new Date();

    this.minDate = new Date()
    
    if(this.minDate.getHours() >= 16){
      this.minDate.setDate(this.minDate.getDate() + 1);
      this.minDate = new Date(this.minDate);
    }
    
  }

  ngOnInit() {

    this.bookingCreates = [] as BookingCreate[];
    this.bookingKey = {} as BookingKey;
    this.isSelect = true;
    this.isCompleted = false;

    this.dataId = this.route.snapshot.params.internalHeaderKey;

    if (this.dataId) {
      this.fetchData();            
    }
    
    this.bookingKey.bookingHeaders = [] as BookingHeader[]; 

    this.cols = [
      {
        field: 'poNbr',
        display: 'string',        
        header: 'PoNo',
      },      
      {
        field: 'planRec',
        display: 'string',        
        header: 'Plan Rec. Date',
      },
      {
        field: 'totalQty',
        display: 'string',        
        header: 'TotalQty',
      },
      {
        field: 'fullPl',
        display: 'string',        
        header: 'Full',
      },
      {
        field: 'con',
        display: 'string',        
        header: 'Con',
      },
      {
        field: 'non',
        display: 'string',        
        header: 'Non',
      },
    ];

    this.truckCols = [
      {
        field: 'truckName',
        display: 'string',        
        header: 'Truck Type',
      },      
      {
        field: 'totalTruck',
        display: 'string',        
        header: 'Total Truck',
      },      
    ];
  }

  initialScreen(){
    this.isSelect = false;
    this.fetchData();
  }

  onCreateTrip() {
    this.confirmDialog = true;
  }

  fetchData() {
    this.truckMasterService.getAll()
    .pipe(
      concatMap(tr => {         
        return this.warehouseService.getAll().pipe(
          map(wh => {
            return {tr,wh}
          })
        );        
      }),
      concatMap(({tr,wh}) => {
        return this.warehouseCapacityService.getAll().pipe(
          map(whCap => {
            return {tr,wh,whCap}
          })
        );        
      }),
      untilDestroyed(this)
    )
    .subscribe({
      next: res => {
        this.trucks = res.tr;
        this.warehouse = res.wh;
        this.warehouseCapacity = res.whCap;
        
        this.warehouse.forEach(x=>x.warehouseDisplay = x.warehouseCode + '-' + x.warehouseName);

        this.bookingKeyService.getBookingKey(this.dataId)
        .pipe(untilDestroyed(this))
        .subscribe((bookingKeyData) => {
          this.bookingKeyDto = bookingKeyData;

          this.bookingKeyDto.bookingDate = new Date(this.bookingKeyDto.bookingDate);

          this.supplierSelected = this.bookingKeyDto.bookingHeaders[0].supCode;
          this.supplierDisplay = this.bookingKeyDto.bookingHeaders[0].supCode + " - " + this.bookingKeyDto.bookingHeaders[0].supName;
          this.supplierName = this.bookingKeyDto.bookingHeaders[0].supName;
          this.internalSupGroupId = this.bookingKeyDto.bookingHeaders[0].internalSupGroupId;
          this.contactName = this.bookingKeyDto.bookingHeaders[0].contactName;
          this.contactPhone = this.bookingKeyDto.bookingHeaders[0].contactTel;
          this.contactEmail = this.bookingKeyDto.bookingHeaders[0].contactEmail;

          this.bookingDate = this.bookingKeyDto.bookingDate;

          let dateNow = new Date();
          
          this.allowToEdit = true;

          if(this.bookingDate.getDate() == dateNow.getDate()){
            this.maxDate = new Date(this.bookingDate);
            this.allowToEdit = false;
          }
          else{
            // this.maxDate.setDate(dateNow.getDate()+30);
          }

          this.bookingKeyDto.bookingHeaders.forEach(hdr => {        
            let bookingCreate = {} as BookingCreate;

            if(hdr.status.toUpperCase() === 'NEW'|| hdr.status.toUpperCase() === 'INTRANSIT' || hdr.status.toUpperCase() === 'APPROVED'){
              // if(this.allowToEdit === false){
              //   this.allowToEdit = true;
              // }
            }
            else{
              this.allowToEdit = false;
            }

            let whse = this.warehouse.find(x=>x.warehouseCode == hdr.warehouseCode);

            bookingCreate.bookingDate = hdr.bookingStart;
            bookingCreate.bookingId = hdr.bookingId;
            bookingCreate.dockDoor = hdr.dockDoor;
            bookingCreate.driverName = hdr.contactName;
            bookingCreate.startTime = new Date(hdr.bookingStart);
            bookingCreate.endTime = new Date(hdr.bookingEnd);
            bookingCreate.warehouseCode = hdr.warehouseCode;
            bookingCreate.warehouseName = whse.warehouseName;
            bookingCreate.warehouseDisplay = whse.warehouseDisplay + ' ' + hdr.merchType;
            bookingCreate.internalDoorId = hdr.internalDoorId;
            bookingCreate.internalHeaderKey = hdr.internalHeaderKey;
            bookingCreate.status = hdr.status;
            bookingCreate.bookingDetail = [] as BookingDetail[];
            bookingCreate.bookingDetail.push(...hdr.bookingDetails);

            bookingCreate.bookingTruck = [] as BookingTruck[];

            hdr.bookingTrucks.forEach(truck => {
              truck.truckName = this.trucks.find(x=>x.internalTruckId == truck.internalTruckId).truckName;
            });

            bookingCreate.bookingTruck.push(...hdr.bookingTrucks);
            
            this.bookingCreates.push(bookingCreate);

            this.onSelectSup();
          });
          
        });
        
      } 
    });

  }

  addPo(){    
    this.poService
    .getByPo(this.inputPoNo,this.supplierSelected,this.bookingDate)
    .pipe(untilDestroyed(this))
    .subscribe(
      {next: (data) =>
        {
          let bh: BookingHeaderDto; 

          this.poDtl = data;

          this.bookingKeyDto.bookingDate = this.bookingDate;

          if(this.bookingKeyDto.bookingHeaders == undefined || this.bookingKeyDto.bookingHeaders == null || this.bookingKeyDto.bookingHeaders.length == undefined){

          }
          else{
            bh = this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == this.poDtl.warehouse_Code);
          }
          
          if(bh == undefined || bh == null){
            bh = {} as BookingHeaderDto;        
            bh.warehouseCode = this.poDtl.warehouse_Code;
            bh.supCode = this.supplierSelected;
            bh.supName = this.supplierName;
            bh.internalSupGroupId = this.internalSupGroupId;
            bh.bookingDetails = [] as BookingDetail[];
            bh.bookingTrucks = [] as BookingTruck[];
            
            let whse = this.warehouse.find(x=>x.warehouseCode == this.poDtl.warehouse_Code);
            let bookingCreate = {} as BookingCreate;

            bookingCreate.warehouseCode = whse.warehouseCode;
            bookingCreate.warehouseName = whse.warehouseName;
            bookingCreate.warehouseDisplay = whse.warehouseDisplay;
            bookingCreate.bookingDate = this.bookingDate;
            bookingCreate.bookingDetail = [] as BookingDetail[];
            bookingCreate.bookingTruck = [] as BookingTruck[];
            
            this.bookingCreates.push(bookingCreate);
            this.bookingKeyDto.bookingHeaders.push(bh);        
          }
          else{
            
          }
          this.addBookingDetail(this.poDtl);
          this.inputPoNo = '';
        },
        error: (error) => 
        {
          this.msgs = [];
          error.Messages.forEach((msg: any) => {                    
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});          
          });
        }
      }
    );
    this.canSaveBooking();
  }

  addBookingDetail(po:PoList): void {     
    // validate duplicate po
    let dtl = this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == po.warehouse_Code).bookingDetails.find(x=>x.poNbr == po.po_Nbr);

    if(dtl == undefined || dtl == null || dtl.poNbr == null){
      dtl = {} as BookingDetail;
      dtl.poNbr = po.po_Nbr;
      dtl.totalQty = po.total_Qty;
      dtl.con = po.con;
      dtl.non = po.non;
      dtl.fullPl = po.full;
      dtl.cubeCon = po.cube_Con;
      dtl.cubeNon = po.cube_Non;
      dtl.cubeFull = po.cube_Full;    
      dtl.planRec = po.plan_Receive_Date;  

      this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == po.warehouse_Code).bookingDetails.push(dtl);      
      this.bookingCreates.find(x=>x.warehouseCode == po.warehouse_Code).bookingDetail.push(dtl);

    }else{
      //console.log('duplicate po detail');
    }

    this.canSaveBooking();
  } 

  deletePo(delPo:any){
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected PO?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
          this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr)).bookingDetail = 
          this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr)).bookingDetail.filter(x=>x.poNbr != delPo.poNbr);
          this.messageService.add({severity:'success', summary: 'Successful', detail: 'PO Deleted', life: 3000});
          this.canSaveBooking();
      }
    });   
  }
 
  onSelectSup(){    
    if(this.contactName.length > 0 && this.contactEmail.length > 0 && this.contactPhone.length > 0){
      this.isSelect = true;
      this.estTimeService
      .getBySupGroupId(this.internalSupGroupId)
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.estTimes = data;
      });  
    
    }
  }

  show(whse:string) {        
    // calculate est time
    this.totalEstTime = 0;
    // this.bookingCreates.find(x=>x.warehouseCode == whse).bookingTruck.forEach(truck=>{
    //   this.totalEstTime += this.estTimes.find(x=>x.internalTruckId == truck.internalTruckId).hourEst * 60;
    //   this.totalEstTime += this.estTimes.find(x=>x.internalTruckId == truck.internalTruckId).minEst;
    // });
    let bookingHdrKeyId = this.bookingCreates.find(x=>x.warehouseCode == whse).internalHeaderKey;
    let bookingId = this.bookingCreates.find(x=>x.warehouseCode == whse).bookingId;

    this.bookingCreates.find(x=>x.warehouseCode == whse).bookingTruck.forEach(truck=>{
      if(this.estTimes.find(x=>x.internalTruckId == truck.internalTruckId) != undefined){
        this.totalEstTime += this.estTimes.find(x=>x.internalTruckId == truck.internalTruckId).hourEst * 60 * truck.totalTruck;
        this.totalEstTime += this.estTimes.find(x=>x.internalTruckId == truck.internalTruckId).minEst * truck.totalTruck;
      }
      else{
        this.totalEstTime = 0;
      }
    });

    this.ref = this.dialogService.open(SlotTimeDtlComponent, {
        header: 'Choose a time slot',
        width: '70%',
        contentStyle: {"max-height": "1000px", "overflow": "auto"},
        baseZIndex: 10000,
        data: { 
          warehouseCode: whse,
          bookingDate: this.bookingDate,
          supplierCode: this.supplierSelected,
          totalEstTime: this.totalEstTime,
          bookingHdrKeyId: bookingHdrKeyId,
          bookingId: bookingId,
          bookingSlot: this.bookingCreates.find(x=>x.warehouseCode == whse),
          internalSupGroupId: this.internalSupGroupId
        }
    });

    this.ref.onClose.subscribe((ret: any) =>{
    
        if (ret) 
        { 
          let door: Door;
          
          this.doorService.getById(ret.EventType).pipe(untilDestroyed(this)).subscribe((data) => { 
            door = data;
            this.messageService.add({severity:'info', summary: 'Door Selected', detail: door.doorName});
            this.messageService.add({severity:'info', summary: 'Start Time Selected', detail: ret.StartTime});
            this.messageService.add({severity:'info', summary: 'End Time Selected', detail: ret.EndTime});
            this.bookingCreates.find(x=>x.warehouseCode == whse).internalDoorId = ret.EventType;
            this.bookingCreates.find(x=>x.warehouseCode == whse).dockDoor = door.doorName;
            this.bookingCreates.find(x=>x.warehouseCode == whse).startTime = new Date(ret.StartTime);
            this.bookingCreates.find(x=>x.warehouseCode == whse).endTime = new Date(ret.EndTime);
           
            this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == whse).internalDoorId = ret.EventType;
            this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == whse).dockDoor = door.doorName;
            this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == whse).bookingStart = ret.StartTime;
            this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == whse).bookingEnd = ret.EndTime;
          
            this.canSaveBooking();

          });            
        }
       
    });
  }

  canSaveBooking(){
     // validate completed booking
     let totalBooking: number = 0;
     let totalCompleted: number = 0;
     totalBooking = this.bookingCreates.length;

     this.bookingCreates.forEach(bookCreate => {
       if(bookCreate.dockDoor != undefined && bookCreate.dockDoor != null){
         totalCompleted++;
       }
     });

     if(totalBooking == totalCompleted){
       this.isCompleted = true;
     }
     else{
      this.isCompleted = false;
     }

  }

  deleteTruck(delTruck:any){
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected Truck?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
          
          this.bookingCreates.find(x=>x.bookingTruck.find(b=>b.internalTruckId == delTruck.internalTruckId)).bookingTruck = 
          this.bookingCreates.find(x=>x.bookingTruck.find(b=>b.internalTruckId == delTruck.internalTruckId)).bookingTruck.filter(x=>x.internalTruckId != delTruck.internalTruckId);

          this.bookingKey.bookingHeaders.find(x=>x.bookingTrucks.find(b=>b.internalTruckId == delTruck.internalTruckId)).bookingTrucks = 
          this.bookingKey.bookingHeaders.find(x=>x.bookingTrucks.find(b=>b.internalTruckId == delTruck.internalTruckId)).bookingTrucks.filter(x=>x.internalTruckId != delTruck.internalTruckId);

          this.messageService.add({severity:'success', summary: 'Successful', detail: 'Truck Deleted', life: 3000});

          this.canSaveBooking();
      }
    });   
  }

  onTruckSelected(whseCode:string,event: any){
    this.bookingCreates.find(x=>x.warehouseCode == whseCode).truckSelected = event.value;    
  }

  addTruck(whseCode:string){
    let whseCreate: BookingCreate;
    let bkTruck: BookingTruck = {} as BookingTruck;
    let update:boolean;
    update = false;

    whseCreate = this.bookingCreates.find(x=>x.warehouseCode == whseCode);

    // check same truck type
    this.bookingCreates.find(x=>x.warehouseCode == whseCode).bookingTruck.forEach(truck=>{
      if(truck.internalTruckId == whseCreate.truckSelected){
        truck.totalTruck = Number.parseInt(truck.totalTruck.toString()) +  Number.parseInt(whseCreate.totalTruck.toString());
        this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == whseCode).bookingTrucks.find(x=>x.internalTruckId == whseCreate.truckSelected).totalTruck = truck.totalTruck;
        update=true;
      }
    });

    if(!update)
    {
      whseCreate = this.bookingCreates.find(x=>x.warehouseCode == whseCode);

      bkTruck.totalTruck = whseCreate.totalTruck;
      bkTruck.internalTruckId = whseCreate.truckSelected;
      bkTruck.truckName = this.trucks.find(x=>x.internalTruckId == whseCreate.truckSelected).truckName;
      
      this.bookingCreates.find(x=>x.warehouseCode == whseCode).bookingTruck.push(bkTruck);
      this.bookingKeyDto.bookingHeaders.find(x=>x.warehouseCode == whseCode).bookingTrucks.push(bkTruck);
    }
    
    this.canSaveBooking();
  }

  onSaveBooking():void{

    this.bookingKeyDto.bookingHeaders = [] as BookingHeaderDto[];
    this.bookingKeyDto.internalKeyId = this.dataId;
    this.bookingKeyDto.bookingDate = this.bookingDate;
    this.bookingKeyDto.userName = this.user.userName;
    this.bookingCreates.forEach(booking => {
      let bookHdr = {} as BookingHeaderDto;      
      bookHdr.warehouseCode = booking.warehouseCode;
      bookHdr.internalDoorId = booking.internalDoorId;
      bookHdr.internalSupGroupId = this.internalSupGroupId;
      bookHdr.supCode = this.supplierSelected;
      bookHdr.bookingId = booking.bookingId;
      bookHdr.internalHeaderKey = booking.internalHeaderKey;      
      bookHdr.supName = this.supplierName;
      bookHdr.firstBookingStart = new Date (booking.startTime);      
      bookHdr.firstBookingEnd = new Date (booking.endTime);
      bookHdr.bookingStart = new Date(booking.startTime);
      bookHdr.bookingEnd = new Date(booking.endTime);
      bookHdr.contactName = this.contactName;
      bookHdr.contactEmail = this.contactEmail;
      bookHdr.contactTel = this.contactPhone;
      bookHdr.bookingDetails = [] as BookingDetail[];
      bookHdr.bookingTrucks = [] as BookingTruck[];

      booking.bookingDetail.forEach(bookingDtl => {
        let bookDtl = {} as BookingDetail;
        bookDtl = bookingDtl;
        bookHdr.bookingDetails.push(bookDtl);
      });

      booking.bookingTruck.forEach(bookingTruck => {
        let bookTruck = {} as BookingTruck;
        bookTruck = bookingTruck;
        bookHdr.bookingTrucks.push(bookTruck);
      });

      this.bookingKeyDto.bookingHeaders.push(bookHdr);
    });

    this.bookingKeyService
        .update(this.bookingKeyDto)
        .pipe(untilDestroyed(this))
        .subscribe((result) => { 
          this.messageService.add({severity:'info', summary: 'Update Booking completed', detail: ""});     
          this.ngOnInit();                                            
        });
    
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

  onDeleteBooking(): void
  {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete Booking for all warehouse?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.bookingHeaderService
        .deleteall(this.bookingCreates[0].internalHeaderKey)
        .pipe(untilDestroyed(this))
        .subscribe((result) => { 
          this.messageService.add({severity:'info', summary: 'Delete Booking completed', detail: ""});     
          this._location.back();
        });    
      }
    });   
  }

  onDeleteBookingWarehouse(bookingDel: BookingCreate): void
  {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this Booking?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.bookingCreates = this.bookingCreates.filter(x=>x.warehouseCode !== bookingDel.warehouseCode);      
        this.messageService.add({severity:'info', summary: 'Delete Booking completed', detail: ""});       
        this.canSaveBooking();
        // this.bookingHeaderService
        // .delete(bookingDel.internalHeaderKey)
        // .pipe(untilDestroyed(this))
        // .subscribe((result) => { 
        //   this.messageService.add({severity:'info', summary: 'Delete Booking completed', detail: ""});     
        //   // this._location.back();
        //   this.ngOnInit();
        // });
      }
    });   
  }

  onApproved(bookingAp: BookingCreate): void{
    this.confirmationService.confirm({
      message: 'Are you sure you want to approved this Booking?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.bookingHeaderService
        .approved(bookingAp.internalHeaderKey)
        .pipe(untilDestroyed(this))
        .subscribe((result) => {
          this.messageService.add({severity:'info', summary: 'Booking approved', detail: ""});
          this.ngOnInit();
        });    
      }
    });
  }
  
}
