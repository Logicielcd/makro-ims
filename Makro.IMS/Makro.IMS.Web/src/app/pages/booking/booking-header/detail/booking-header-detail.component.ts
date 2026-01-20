import { Location } from '@angular/common';
import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
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
import { PoService } from '@core/services/booking/po.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { EstTimeService } from '@core/services/master/est-time.service';
import { DoorService } from '@core/services/master/door.service';
import { BookingKeyService } from '@core/services/booking/booking-key.service';

/* model */
import { BookingKey,BookingHeader,BookingDetail, BookingTruck,BookingKeyDto,BookingHeaderDto } from '@core/models/booking/booking-header.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { SupplierGroup } from '@core/models/master/supplier.model';
import { PoList } from '@core/models/booking/po.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { User } from '@core/models/account/user.model';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { EstTime } from '@core/models/master/est-time.model';

import { ActivatedRoute,  Router } from '@angular/router';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { Observable, catchError, combineLatest, map, of, switchMap } from 'rxjs';
import { SlotTimeModalComponent } from '@theme/components/modal/slot-time-modal/slot-time-modal.component';
import { PoDialogComponent } from '@theme/components/modal/po-dialog/po-dialog.component';
import { TruckCap, TruckRule } from '@core/models/master/truck-cap.model';
import { Operation } from '@core/models/master/operation.model';
import { OperationService } from '@core/services/master/operation.service';
import { environment } from '@environments/environment';
import { BackhaulBooking } from '@core/models/booking/backhaul-booking';
import { DcDelayBooking } from '@core/models/booking/dcdelay-booking';

@UntilDestroy()
@Component({
  selector: 'app-booking-header-detail',
  templateUrl: './booking-header-detail.component.html',
  styleUrls: ['./booking-header-detail.component.scss'],
  encapsulation: ViewEncapsulation.None, 
})
export class BookingHeaderDetailComponent implements OnInit {

  // Auth
  isAdmin: boolean = false;

  title = 'Booking Detail'
  user: User;
  msgs: Message[] = [];
  dataId: number;

  menuItems$: Observable<MenuItem[]>;
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
  
  supplierGroup: SupplierGroup;
  
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
  canDeleteAll: boolean = false;

  // po variable  
  poDtl: PoList;
  inputPoNo: string;
  cols: any[];
  colsCs: any[];

  // truck variable
  bookingTruck: BookingTruck;
  trucks: TruckMaster[];
  truckSelected: number;
  truckDriver: string;
  truckLicense: string;
  truckCols: any[];

  truckRules: TruckRule[];

  // estTime
  estTimes: EstTime[];
  totalEstTime: number;

  bookingKeyDto: BookingKeyDto;

  allowToEdit: boolean;
  isApproved: boolean;

  minDate: Date;
  maxDate: Date;

  selectedPoList: string[];

  truckCaps: TruckCap[];

  operationTypes: Operation[];

  isBh: boolean;
  isDcDelay: boolean;

  canUpdateBooking: boolean = false;

  showRemark: boolean;
  cancelBookingHdr: BookingHeaderDto;
  remarkCancel: string;
  allCancel: boolean;

  nextShift: Date;
  minBookingDate: Date;
  cutoffTime: Date;
  overCutoff: boolean;

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
    private router: Router,
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
    private estTimeService: EstTimeService,
    private doorService: DoorService,
    private bookingKeyService: BookingKeyService,
    private _location: Location,    
    private operationService: OperationService
  ) {

    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.warehousecapacity)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));

    this.canModify = true;
    this.canDeleteAll = false;
    this.showRemark = false;

    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
      
    this.menuItems$ = this.getMenuItems();
    
   // this.bookingDate.setDate(this.bookingDate.getDate() + 1);
    this.user = this.authService.getUser();
    if(this.user.userType.toLowerCase() !== 'sup' && this.user.userType.toLowerCase() !== 'suptran' && this.user.userType.toLowerCase() !== 'bh'){
      this.isAdmin = true;
    }
    else{
      this.isAdmin = false;
    }

    this.minDate = new Date();
    this.nextShift = new Date();
    this.cutoffTime = new Date();
    this.minBookingDate = new Date();
    this.overCutoff = false;
  }

  getMenuItems(): Observable<MenuItem[]> {
    const menus: MenuItem[] = [      
      {
        label: 'Save booing',
        icon: 'pi pi-save',
        disabled: !this.isCompleted || this.allCancel,
        command: () => {
            this.onSaveBooking();
        }
      },
      {
          label: 'Delete all booking',
          icon: 'pi pi-trash',
          disabled: this.canDeleteAll || this.allCancel,
          command: () => {
              this.onDeleteBooking();
          }
      },
    ];

    return of(menus);
  }

  ngOnInit() {
    this.initializeVariables();
    const decodedId = atob(this.route.snapshot.params.internalHeaderKey);
    this.dataId = +decodedId;

    if (this.dataId) {
      this.fetchData();
      this.menuItems$ = this.getMenuItems();
    }
    
    this.bookingKey.bookingHeaders = [] as BookingHeader[]; 
    this.initializeColumns();

  }

  initializeVariables() {
    this.isLoading = false;
    this.isCompleted = false;
    this.isSelect = false;
    this.isBh = false;
    this.isDcDelay = false;
    this.bookingCreates = [] as BookingCreate[];
    this.bookingKey = {} as BookingKey;
    this.selectedPoList = [] as string[];
    this.bookingKey.bookingHeaders = [] as BookingHeader[];
    this.allCancel = false;
  }

  initializeColumns() {
    this.cols = this.getCommonColumns([          
        { field: 'totalQty', display: 'string', header: 'TotalQty' },
        { field: 'fullPl', display: 'string', header: 'Full' },
        { field: 'con', display: 'string', header: 'Con' },
        { field: 'non', display: 'string', header: 'Non' },
        { field: 'weight', display: 'string', header: 'Weight' },
        { field: 'remark', display: 'textbox', header: 'Remark' }
    ]);

    this.colsCs = this.getCommonColumns([          
        { field: 'totalQty', display: 'string', header: 'TotalQty' },
        { field: 'fullPl', display: 'string', header: 'Full' },
        { field: 'fullCs', display: 'string', header: 'Full CS' },
        { field: 'halfPl', display: 'string', header: 'Half' },
        { field: 'halfCs', display: 'string', header: 'Half CS' },
        { field: 'con', display: 'string', header: 'Con' },
        { field: 'non', display: 'string', header: 'Non' },
        { field: 'weight', display: 'string', header: 'Case' },
        { field: 'remark', display: 'textbox', header: 'Remark' }
    ]);

    this.truckCols = [
        { field: 'truckName', display: 'string', header: 'Truck Type' },
        { field: 'totalTruck', display: 'string', header: 'Total Truck' }
    ];
  }

  getCommonColumns(additionalColumns: any[]) {
      const baseColumns = [
          { field: 'alert', display: 'icon', header: 'Alert' },
          { field: 'merchType', display: 'string', header: 'Zone' },
          { field: 'poNbr', display: 'string', header: 'PoNo' },
          { field: 'planRec', display: 'date', header: 'Plan Delivery Date' },
          { field: 'expireDate', display: 'date', header: "Expire Date"}
      ];
      return [...baseColumns, ...additionalColumns];
  }

  initialScreen(){
    this.isSelect = false;
    this.isCompleted = false;
    this.fetchData();
  }

  onCreateTrip() {
    this.confirmDialog = true;
  }

  fetchData() {
    this.isLoading = true;
  
    combineLatest([
      this.warehouseService.getAll(),
      this.truckMasterService.getAll(),
      this.truckMasterService.getTruckCapAll(),
      this.truckMasterService.getTruckRuleAll(),
      this.operationService.getAll()
    ])
    .pipe(
      untilDestroyed(this),
      switchMap(([whseData, truckData, truckCapData, truckRuleData, operationData]) => {
        this.warehouse = whseData.map(x => ({
          ...x,
          warehouseDisplay: `${x.warehouseCode}-${x.warehouseName}`
        }));
        this.trucks = truckData;
        this.truckCaps = truckCapData;
        this.truckRules = truckRuleData;
        this.operationTypes = operationData;
  
        return this.bookingKeyService.getBookingKey(this.dataId);
      }),
      switchMap(bookingKeyData => {
        this.bookingKeyDto = bookingKeyData;
        this.bookingKeyDto.bookingDate = new Date(this.bookingKeyDto.bookingDate);
        this.bookingDate = this.bookingKeyDto.bookingDate;
        this.setSupplierDetails();
        this.processBookingHeaders();
  
        return this.supplierService.getSupGroup(this.bookingKeyDto.bookingHeaders[0].internalSupGroupId);
      }),
      map(supGrp => {
        this.supplierGroup = supGrp;
        this.supplierGroup.isVip = this.supplierGroup.isVip === 'Y' ? 'Fix time slot' : '';
      }),
      catchError(error => {
        this.handleErrors(error);
        return []; // ส่งคืน observable ที่ว่างเปล่าเพื่อจบการทำงาน
      })
    )
    .subscribe({
      next: () => {
        this.isLoading = false;
        this.onSelectSup();
        this.canApproved();
        this.deleteAll();
        this.canSaveBooking();
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }
  
  // ฟังก์ชันย่อยสำหรับจัดการข้อมูล supplier
  setSupplierDetails() {
    const bookingHeader = this.bookingKeyDto.bookingHeaders[0];
    this.supplierSelected = bookingHeader.supCode;
    this.supplierDisplay = `${bookingHeader.supCode} - ${bookingHeader.supName}`;
    this.supplierName = bookingHeader.supName;
    this.internalSupGroupId = bookingHeader.internalSupGroupId;
    this.contactName = bookingHeader.contactName;
    this.contactPhone = bookingHeader.contactTel;
    this.contactEmail = bookingHeader.contactEmail;
    this.bookingDate = new Date(this.bookingKeyDto.bookingDate);
  }
  
  // ฟังก์ชันย่อยสำหรับประมวลผล booking headers
  processBookingHeaders() {    
    this.allowToEdit = true;
  
    this.bookingKey.bookingDate = this.bookingKeyDto.bookingDate;
    this.bookingKey.internalKeyId = this.bookingKeyDto.internalKeyId;
    this.bookingKey.bookingHeaders = [] as BookingHeader[];

    this.bookingKeyDto.bookingHeaders = this.bookingKeyDto.bookingHeaders.sort((a,b) => a.bookingId.localeCompare(b.bookingId));    

    this.bookingKeyDto.bookingHeaders.forEach(hdr => {
      if (!['NEW','EDIT','OVERCAP','OVERCUTOFF', 'INTRANSIT', 'APPROVED'].includes(hdr.status.toUpperCase())) {
        this.allowToEdit = false;
      }      

      let bkHeader = {} as BookingHeader;
      bkHeader.internalHeaderKey = hdr.internalHeaderKey;
      bkHeader.warehouseCode = hdr.warehouseCode;
      bkHeader.companyCode = hdr.companyCode;
      bkHeader.postPoned = hdr.postPoned;
      bkHeader.bookingStart = hdr.bookingStart;
      bkHeader.bookingEnd = hdr.bookingEnd;
      bkHeader.backHaul = hdr.backHaul;
      bkHeader.bookingDetails = [] as BookingDetail[];
      bkHeader.isDelay = hdr.isDelay === 1 ? true : false;
      bkHeader.remarkDelay = hdr.remarkDelay;
      bkHeader.remark = hdr.remark;

      hdr.bookingDetails.forEach(dtl => {        
        dtl.alert = dtl.isDelay ? 'Y' : 'N';
        dtl.delayReason = dtl.delayReason;          
        dtl.isPostPoned = dtl.delayReason === 'DC Delay' ? true : false;  
        dtl.expireDate = new Date(dtl.expireDate);

        dtl.planRec = new Date(dtl.planRec);
        // re-calculate delay
        if(this.bookingKeyDto.bookingDate > dtl.planRec){
          if(dtl.isDelay === false){
            dtl.isDelay = true;
            dtl.alert = 'Y';
          }
        }

        bkHeader.bookingDetails.push(dtl);        
      });

      this.bookingKey.bookingHeaders.push(bkHeader);

      const whse = this.warehouse.find(x => x.warehouseCode === hdr.warehouseCode);
      if (whse) {
        
        this.calculateCutoff(whse);
        
        // if(this.bookingKey.bookingDate < this.minDate && (this.user.userType == 'SUP' || this.user.userType == 'SUPTRAN'
        //   || this.user.userType == 'BH')){
        //  this.allowToEdit = false;
        // }        

        let totalQty = 0;
        hdr.bookingDetails.forEach((dtl) => totalQty += dtl.weight);
        let bookingCreate = {} as BookingCreate;
        Object.assign(bookingCreate, {
          bookingDate: hdr.bookingStart,
          bookingId: hdr.bookingId,
          dockDoor: hdr.dockDoor,
          driverName: hdr.contactName,
          startTime: new Date(hdr.bookingStart),
          endTime: new Date(hdr.bookingEnd),
          warehouseCode: hdr.warehouseCode,
          warehouseName: whse.warehouseName,
          whseCap: whse.capUom,
          warehouseDisplay: `${whse.warehouseDisplay}`,
          internalDoorId: hdr.internalDoorId,
          internalHeaderKey: hdr.internalHeaderKey,
          status: hdr.status,
          merchType: hdr.merchType,
          merchTypeDisplay: this.operationTypes.find(x=>x.operationName == hdr.merchType).description,
          companyCode: whse.companyCode,
          postponed: hdr.postPoned,
          backHaul: hdr.backHaul,
          firstTimeStart: new Date(hdr.firstBookingStart),
          firstTimeEnd: new Date(hdr.firstBookingEnd),
          firstUser: hdr.firstUserStamp,
          modDate: new Date(hdr.modDate),
          userStamp: hdr.userStamp,
          bookingDetail: [...hdr.bookingDetails],
          bookingTruck: hdr.bookingTrucks.map(truck => {
            const truckInfo = this.trucks.find(x => x.internalTruckId === truck.internalTruckId);
            return { ...truck, truckName: truckInfo?.truckName };
          }),
          allApproved: false,
          isLate: hdr.bookingDetails.find(x=>x.delayReason !== null && x.delayReason !== undefined && x.delayReason.length > 0) ? true : false,
          isDelay: hdr.isDelay,
          totalQty: totalQty,
          remarkDelay: hdr.remarkDelay,
          remark: hdr.remark,
          overCutoff: this.overCutoff,
          nextShift: this.nextShift,
          minBookingDateTime: this.minBookingDate
        });

        this.bookingCreates.push(bookingCreate);

        if(this.bookingKeyDto.bookingHeaders.filter(x=>x.status != 'CANCEL').length > 0){
          this.allCancel = false;
        }
        else{
          this.allCancel = true;
        }
      }
    });
  }
  
  private calculateCutoff(whse: Warehouse) {
    let curDate: Date = new Date();
    this.nextShift = new Date();
    this.minBookingDate = new Date();
    this.overCutoff = false;

    if (whse.advanceBookingDay > -1) {
      if (whse.firstTimeOfDay > whse.endTimeOfDay) {
        if (this.nextShift.getHours() >= whse.firstTimeOfDay) {
          this.nextShift.setDate(this.nextShift.getDate() + 1);
          this.nextShift.setHours(whse.firstTimeOfDay);
          this.nextShift.setMinutes(0);
          this.nextShift.setSeconds(0);
        }
        else {
          this.nextShift.setHours(whse.firstTimeOfDay);
          this.nextShift.setMinutes(0);
          this.nextShift.setSeconds(0);
        }

      }
      else {
        this.nextShift.setDate(this.nextShift.getDate() + 1);
        this.nextShift.setHours(whse.firstTimeOfDay);
        this.nextShift.setMinutes(0);
        this.nextShift.setSeconds(0);
      }
      curDate.setHours(curDate.getHours() + whse.advanceBookingDay);
      //console.log('curDate:',curDate);
      if (curDate >= this.nextShift) {
        this.overCutoff = true;
        this.nextShift.setDate(this.nextShift.getDate() + 1);
      }
    }
    else {
      // minBookingDate.setMinutes(minBookingDate.getMinutes() + whse.advanceBookingPeriod);
      this.nextShift.setMinutes(this.nextShift.getMinutes() + whse.advanceBookingPeriod);
      this.minBookingDate.setDate(this.nextShift.getDate());
    }

    this.minBookingDate.setDate(this.nextShift.getDate());
    if (whse.firstTimeOfDay > whse.endTimeOfDay) {
      this.minBookingDate.setDate(this.minBookingDate.getDate() + 1);
    }

    this.minBookingDate.setHours(0, 0, 0);

    this.minDate = curDate;
/*
    console.log('cal nextshift');
    console.log('minBookingDate:', this.minBookingDate);
    console.log('nextShift:', this.nextShift);
    console.log('mindate', this.minDate);    
    console.log('----');
*/
  }

  // ฟังก์ชันย่อยสำหรับจัดการข้อผิดพลาด
  handleErrors(error: any) {
    this.msgs = [];
    if (error.Messages && Array.isArray(error.Messages)) {
      error.Messages.forEach((msg: string) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: msg, life: 10000 });
      });
    } else {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'An unexpected error occurred.', life: 10000 });
    }
    this.isLoading = false;
  }
  
  addPo(item: any){

    if(item.inputPoNo.includes (',')){
      var poList = item.inputPoNo.split(',');
      poList.forEach(po => {
        this.addPoData(po.trim(),item);
      });
    }
    else{
      this.addPoData(item.inputPoNo.trim(),item);
    }
    //this.addPoData(item.inputPoNo,item);
  }

  addPoData(po: string,item: any){
    this.isLoading = true;
    this.poService
    .getByPo(po, this.supplierSelected, this.bookingDate)
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.poDtl = data;
        this.bookingKey.bookingDate = this.bookingDate;          
        let dateNow: Date = new Date();
        this.poDtl.plan_Receive_Date = new Date(this.poDtl.plan_Receive_Date);
        let planRecDate: Date = new Date(data.plan_Receive_Date);
        let bookingBefore: number = + environment.bookingBefore;        
        this.poDtl.alert = planRecDate < this.bookingDate ? 'Y' : 'N';

        // ตรวจสอบวันที่รับสินค้าตามแผนกับวันที่จอง
        if (planRecDate.getDate() !== this.bookingDate.getDate() && dateNow.getHours() < bookingBefore) {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: `Can't book this PO because booking date and plan receive date are different. Please book after ${bookingBefore}.00 O'Clock \r\n ไม่สามารถนัดหมายได้ เนื่องจากวันนัดหมายกับวันในใบสั่งซื้อไม่ตรงกัน กรุณาจองเวลาหลัง ${bookingBefore}.00 น.`,
            life: 10000
          });
          this.isLoading = false;
          return;
        }

        // ตรวจสอบว่ามี bookingHeaders หรือไม่
        let bh: BookingHeader | undefined;
        let whse = this.warehouse.find(x => x.warehouseCode === this.poDtl.warehouse_Code && x.companyCode === this.poDtl.company_Code);
        
        // let minBookingDate = new Date();
        // let nextShift = new Date();
        // let curDate: Date = new Date();
        
        this.calculateCutoff(whse);

        // คำนวณวันที่สามารถ booking ได้
        // if(whse.advanceBookingDay > -1){
        //   if(whse.firstTimeOfDay > whse.endTimeOfDay){
        //     if (nextShift.getHours() >= whse.firstTimeOfDay){
        //       nextShift.setDate(nextShift.getDate() + 1);
        //       nextShift.setHours(whse.firstTimeOfDay);
        //       nextShift.setMinutes(0);
        //       nextShift.setSeconds(0);
        //     }
        //     else{
        //       nextShift.setHours(whse.firstTimeOfDay);
        //       nextShift.setMinutes(0);
        //       nextShift.setSeconds(0);
        //     }
        //     // if(minBookingDate.getHours() >= whse.firstTimeOfDay){              
        //     //   //console.log('next 2');
        //     //   minBookingDate.setDate(minBookingDate.getDate() + 2 + whse.advanceBookingDay);
        //     // }else{
        //     //   //console.log('next 1');
        //     //   minBookingDate.setDate(minBookingDate.getDate() + 1 + whse.advanceBookingDay);
        //     // }
        //     // if(minBookingDate.getHours() >= whse.endTimeOfDay){
        //     //  // console.log('add another 1 day');
        //     //   minBookingDate.setDate(minBookingDate.getDate() + 1);
        //     // }
        //   }
        //   else{
        //     //minBookingDate.setDate(minBookingDate.getDate() + whse.advanceBookingDay);
        //     nextShift.setDate(nextShift.getDate() + 1);
        //     nextShift.setHours(whse.firstTimeOfDay);
        //     nextShift.setMinutes(0);
        //     nextShift.setSeconds(0);
        //   }
        //   curDate.setHours(curDate.getHours() + whse.advanceBookingDay)
        //   //console.log('curDate:',curDate);
        //   if(curDate >= nextShift){
        //     overCutoff = true;
        //     nextShift.setDate(nextShift.getDate() + 1);
        //   }
        // }
        // else{
        //   //minBookingDate.setMinutes(minBookingDate.getMinutes() + whse.advanceBookingPeriod);
        //   nextShift.setMinutes(nextShift.getMinutes() + whse.advanceBookingPeriod);
        //   minBookingDate.setDate(nextShift.getDate());
        // }

        // minBookingDate.setDate(nextShift.getDate());
        // if(whse.firstTimeOfDay > whse.endTimeOfDay){
        //   minBookingDate.setDate(minBookingDate.getDate() + 1);
        // }

        bh = this.bookingKey.bookingHeaders.find(x=>x.internalHeaderKey == item.internalHeaderKey);

        let bookingCreate = this.bookingCreates.find(x=>x.internalHeaderKey == bh.internalHeaderKey)

        bookingCreate.nextShift = this.nextShift;
        bookingCreate.minBookingDateTime = this.minBookingDate;
        bookingCreate.overCutoff = this.overCutoff;

        bookingCreate.dockDoor = null;

        this.addBookingDetail(this.poDtl,bh);
        this.selectedPoList.push(this.inputPoNo);
        this.inputPoNo = '';
        this.isLoading = false;
      },
      error: (error) => this.handleErrors(error)
    });
  }

  addBookingDetail(po: PoList,bh: BookingHeader): void {
    // หา warehouse ที่ตรงกับ po.warehouse_Code
    const whse = this.warehouse.find(x => x.warehouseCode === po.warehouse_Code);
    if (!whse) return; // ถ้าไม่เจอ warehouse ก็หยุดทำงาน
  
    // ตรวจสอบว่ามี po.Nbr ที่ตรงกันใน bookingDetails หรือไม่
    let dtl = bh.bookingDetails.find(x => x.poNbr === po.po_Nbr);

    if (!dtl) {
      dtl = {} as BookingDetail;
      Object.assign(dtl, {
        poNbr: po.po_Nbr,
        totalQty: po.total_Qty,
        con: po.con,
        non: po.non,
        fullPl: po.full,
        cubeCon: po.cube_Con,
        cubeNon: po.cube_Non,
        cubeFull: po.cube_Full,
        planRec: new Date(po.plan_Receive_Date),
        postponed: po.postPoned,
        merchType: po.merch_Type,
        weight: po.weight,
        halfPl: po.half,
        halfCs: po.half_Cs,
        fullCs: po.full_Cs,
        alert: po.alert,
        expireDate: new Date(po.expire_Date),
        delayReason: po.delay_Reason,
        isPostPoned: po.postPoned === 'N' ? false : true
      });
  
      // เพิ่มข้อมูลใน bookingHeaders และ bookingCreates
      bh.bookingDetails.push(dtl);
      
      let bookingCreate = this.bookingCreates.find(x=>x.internalHeaderKey == bh.internalHeaderKey)

      bookingCreate?.bookingDetail.push(dtl);
     
      this.updateMerchtype(whse,bookingCreate);
      this.updateDelayDelivery(whse,bookingCreate);
    } else {
      // ถ้าซ้ำไม่ทำอะไร
      //console.log('duplicate po detail');
    }
  }
  
  updateDelayDelivery(whse:any,bkCreate: BookingCreate){
    // let nextShift = new Date();
    // let minBookingDate = new Date();
        
    // // คำนวณวันที่สามารถ booking ได้
    // if(whse.advanceBookingDay >= 0){
    //   if(whse.firstTimeOfDay > whse.endTimeOfDay){
    //     if (nextShift.getHours() >= whse.firstTimeOfDay){
    //       nextShift.setDate(nextShift.getDate() + 1);
    //       nextShift.setHours(whse.firstTimeOfDay);
    //       nextShift.setMinutes(0);
    //       nextShift.setSeconds(0);
    //     }
    //     else{
    //       nextShift.setHours(whse.firstTimeOfDay);
    //       nextShift.setMinutes(0);
    //       nextShift.setSeconds(0);
    //     }
    //     // if(minBookingDate.getHours() >= whse.firstTimeOfDay){
    //     //   minBookingDate.setDate(minBookingDate.getDate() + 2 + whse.advanceBookingDay);
    //     // }else{
    //     //   minBookingDate.setDate(minBookingDate.getDate() + 1 + whse.advanceBookingDay);
    //     // }
    //   }
    //   else{

    //     nextShift.setDate(nextShift.getDate() + 1);
    //     nextShift.setHours(whse.firstTimeOfDay);
    //     nextShift.setMinutes(0);
    //     nextShift.setSeconds(0);
    //     minBookingDate.setDate(minBookingDate.getDate() + whse.advanceBookingDay);
    //   }
    // }
    // else{
    //   nextShift.setMinutes(nextShift.getMinutes() + whse.advanceBookingPeriod);
    //   minBookingDate.setMinutes(minBookingDate.getMinutes() + whse.advanceBookingPeriod);
    // }

    // minBookingDate = nextShift;

    // console.log('minBookingDate:',minBookingDate);
    // console.log('nextShift:',nextShift);

    let checkLate = false;

    let bDate = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate());
    
    bkCreate.bookingDetail.forEach(po => {      
      po.planRec = new Date(po.planRec);
      let pDate = new Date(po.planRec.getFullYear(),po.planRec.getMonth(),po.planRec.getDate())
      if(bDate > pDate){
        checkLate = true;
      }
    });

    bkCreate.isLate = checkLate;
    bkCreate.isDelay = checkLate;
    
  }

  updateMerchtype(whse:any, bkCreate: BookingCreate){
    let totalQty = 0;
        
    bkCreate.bookingDetail.forEach((dtl)=> {
      totalQty += dtl.weight;      
    });

    bkCreate.totalQty = totalQty;

    if (whse.capUom == 'CS'){
      let qtyType = '';
      let totalPl = 0;
      let totalHf = 0;
      let totalCon = 0;
      let totalNon = 0;
      bkCreate?.bookingDetail.forEach((dtl)=> {
        totalPl += dtl.fullPl + (dtl.halfPl / 2);
        totalCon += dtl.con;
        totalNon += dtl.non;
        totalHf += dtl.halfPl;
      });
      
      if(bkCreate.bookingTruck.length > 0){        
        let maxTruck = '000';
        bkCreate.bookingTruck.forEach(t => {
          let t1 = this.trucks.find(x=>x.internalTruckId == t.internalTruckId);
          
          if(t1.sequence > maxTruck){
            maxTruck = t1.sequence;
            bkCreate.truckSelected = t1.internalTruckId;
          }
        });        
      }

      if(bkCreate.truckSelected !== undefined && bkCreate.truckSelected !== 0){
        let truck = this.trucks.find(x=>x.internalTruckId == bkCreate.truckSelected);        
        let truckCap = this.truckCaps.find(x=>x.internalTruckId == bkCreate.truckSelected && x.warehouseCode == bkCreate.warehouseCode);   
      
        if(truckCap !== undefined && totalPl >= truckCap.fullPl){
          qtyType = 'FULL';
        }
        else if(totalPl > 0 || totalNon > 0 || totalCon > 0 || totalHf > 0){
          qtyType = totalCon > totalNon ? 'CON' : 'NON';
        }
        else{
          qtyType = '';
        }
        
        var truckRule = this.truckRules.find(x => x.internalTruckId.split('|').includes(truck.internalTruckId.toString()) 
        && x.warehouse == bkCreate.warehouseCode
        && x.qtyType.split('|').includes(qtyType));

        if (truckRule != undefined)
        {
          bkCreate.merchType = truckRule.operationType;
          bkCreate.merchTypeDisplay = this.operationTypes.find(x=>x.warehouseCode == bkCreate.warehouseCode && x.operationName == truckRule.operationType).description;
        }
        else {
          const groupedTotals = bkCreate.bookingDetail.reduce((accumulator, current) => {
            // ตรวจสอบว่ามี item นี้ใน accumulator แล้วหรือยัง
            if (accumulator[current.merchType]) {
              // ถ้ามีแล้ว ให้บวก qty เพิ่มเข้าไป
              accumulator[current.merchType] += current.totalQty;
            } else {
              // ถ้ายังไม่มี ให้กำหนดค่าเริ่มต้นเป็น qty ของ item นั้น
              accumulator[current.merchType] = current.totalQty;
            }
            return accumulator;
          }, {} as Record<string, number>);
          
          const groupedArray = Object.entries(groupedTotals).map(([item, qty]) => ({ item, qty }));
          // เรียงลำดับจากมากไปน้อยแล้วเลือกค่าแรก
          const maxItem = groupedArray.sort((a, b) => b.qty - a.qty)[0];
    
          bkCreate.merchType = maxItem.item;
          bkCreate.merchTypeDisplay = this.operationTypes.find(x=>x.warehouseCode == bkCreate.warehouseCode && x.operationName == maxItem.item).description;
        }
      }
    }
    else{      
      const groupedTotals = bkCreate.bookingDetail.reduce((accumulator, current) => {
        // ตรวจสอบว่ามี item นี้ใน accumulator แล้วหรือยัง
        if (accumulator[current.merchType]) {
          // ถ้ามีแล้ว ให้บวก qty เพิ่มเข้าไป
          accumulator[current.merchType] += current.totalQty;
        } else {
          // ถ้ายังไม่มี ให้กำหนดค่าเริ่มต้นเป็น qty ของ item นั้น
          accumulator[current.merchType] = current.totalQty;
        }
        return accumulator;
      }, {} as Record<string, number>);
      
      const groupedArray = Object.entries(groupedTotals).map(([item, qty]) => ({ item, qty }));
      // เรียงลำดับจากมากไปน้อยแล้วเลือกค่าแรก
      const maxItem = groupedArray.sort((a, b) => b.qty - a.qty)[0];

      bkCreate.merchType = maxItem.item;
      bkCreate.merchTypeDisplay = this.operationTypes.find(x=>x.warehouseCode == bkCreate.warehouseCode && x.operationName == maxItem.item).description;
    }
    
  }

  deletePo(delPo:any,item: any){
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected PO?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
          this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr)).bookingDetail = 
          this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr)).bookingDetail.filter(x=>x.poNbr != delPo.poNbr);
          this.messageService.add({severity:'success', summary: 'Successful', detail: 'PO Deleted', life: 10000});

          this.bookingKeyDto.bookingHeaders.find(x=>x.bookingDetails.find(b=>b.poNbr == delPo.poNbr)).bookingDetails = 
          this.bookingKeyDto.bookingHeaders.find(x=>x.bookingDetails.find(b=>b.poNbr == delPo.poNbr)).bookingDetails.filter(x=>x.poNbr != delPo.poNbr);


          this.selectedPoList.splice(this.selectedPoList.indexOf(delPo.poNbr),1);

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
      },(error)=>{
        this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 10000});
          });
      });  
    
    }
  }

  show(whseCode: string, companyCode: string, merchType: string,selectedBookingId: string) {
    this.totalEstTime = 0;
  
    const whse = this.warehouse.find(x => x.warehouseCode === whseCode);
    const booking = this.getBooking(whseCode, companyCode, merchType,selectedBookingId);
    const bookingId = booking?.bookingId;
    const bookingHdrKeyId = booking?.internalHeaderKey;
  
    if (whse) {
      this.calculateEstTime(whse, merchType, companyCode,bookingId);
    }
  
    const modalComponent =  SlotTimeModalComponent;
  
    this.ref = this.dialogService.open(modalComponent, {
      header: 'Choose a time slot of ' + merchType,
      width: '1280px',
      contentStyle: { "max-height": "1000px", "overflow": "auto" },
      baseZIndex: 10000,
      data: {
        warehouseCode: whseCode,
        bookingDate: this.bookingDate,
        supplierCode: this.supplierSelected,
        totalEstTime: this.totalEstTime,
        bookingHdrKeyId,
        bookingId,
        bookingSlot: booking,
        internalSupGroupId: this.internalSupGroupId,
        //totalWeight: this.totalWeight
      }
    });
  
    this.ref.onClose.subscribe((ret: any) => {
      if (ret) {
        this.handleModalClose(ret, whse, whseCode, companyCode, merchType);
      }
    });
  }
  
  getBooking(whseCode: string, companyCode: string, merchType: string, selectedBookingId: string) {
    return this.bookingCreates.find(x =>
      x.warehouseCode === whseCode &&
      (x.merchType === merchType || x.companyCode === companyCode || x.warehouseCode === whseCode) &&
      x.bookingId === selectedBookingId
    );
  }
  
  calculateEstTime(whse: any, merchType: string, companyCode: string,bookingId: string) {
    const relevantBooking = this.bookingCreates.find(x =>
      x.warehouseCode === whse.warehouseCode &&
      (whse.warehouseLevel === 'D' ? x.merchType === merchType :
        whse.warehouseLevel === 'C' ? x.companyCode === companyCode :
          true)
      && x.bookingId === bookingId
    );
  
    relevantBooking?.bookingTruck.forEach(truck => {
      const estTime = this.estTimes.find(x => x.internalTruckId == truck.internalTruckId && x.warehouseCode == whse.warehouseCode
        && x.operationType.split('|').includes(merchType)
      );
      
      if (estTime) {
          this.totalEstTime += estTime.hourEst * 60 + estTime.minEst;
      } else {
          this.totalEstTime = 0; // Reset to 0 if estTime not found
      }
    });

  }
  
  handleModalClose(ret: any, whse: any, whseCode: string, companyCode: string, merchType: string) {
    if (ret.EventType) {
      this.doorService.getById(ret.EventType).pipe(untilDestroyed(this)).subscribe(data => {
        this.updateDoorInfo(data, whse, whseCode, companyCode, merchType, ret);
      }, error => this.handleErrors(error));
    } else {
      this.updateDoorInfo(null, whse, whseCode, companyCode, merchType, ret);
    }
  }
  
  updateDoorInfo(door: any, whse: any, whseCode: string, companyCode: string, merchType: string, ret: any) {
    const doorId = door?.internalDoorId || 0;
    const doorName = door?.doorName || '-';
  
    this.messageService.add({
      severity: 'success',
      summary: 'Slot time selected',
      detail: doorName || '-'
    });
  
    const levelCheck = (x: any) => {
      if (whse.warehouseLevel === 'D') return x.merchType === merchType;
      if (whse.warehouseLevel === 'C') return x.companyCode === companyCode;
      return true;
    };
  
    
    const booking = this.bookingCreates.find(x => x.warehouseCode === whseCode && x.bookingId === ret.BookingId && levelCheck(x));
    const bookingHeader = this.bookingKeyDto.bookingHeaders.find(x => x.warehouseCode === whseCode && x.bookingId === ret.BookingId && levelCheck(x));
  
    if (booking && bookingHeader) {
      booking.internalDoorId = doorId;
      booking.dockDoor = '-';
      booking.startTime = new Date(ret.StartTime);
      booking.endTime = new Date(ret.EndTime);
      booking.status = ret.OverCap ? 'OVERCAP' : '';
      
      bookingHeader.internalDoorId = doorId;
      bookingHeader.dockDoor = '-';
      bookingHeader.bookingStart = new Date(ret.StartTime);
      bookingHeader.bookingEnd = new Date(ret.EndTime);
      bookingHeader.status = ret.OverCap ? 'OVERCAP' : '';
    }
  
    this.canSaveBooking();
    this.deleteAll();
  }
  
  canSaveBooking(){

    this.msgs = [];
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
     
    this.bookingCreates.forEach((bk)=>{
      bk.bookingDetail.forEach((bd)=>{
        bk.bookingDate = new Date(this.bookingDate); //new Date(bk.bookingDate);
        bd.planRec = new Date(bd.planRec);
        bd.expireDate = new Date(bd.expireDate);
        bd.expireDate.setHours(0);
        bd.expireDate.setMinutes(0);
        
        if(bd.expireDate <= this.bookingDate){
          this.isCompleted = false;

          this.msgs.push({
            severity: 'error',
            summary: 'Error',
            detail: `Can't book PO : ${bd.poNbr} expired. \r\n ไม่สามารถนัดหมายได้ เนื่องจาก PO : ${bd.poNbr} หมดอายุ`,            
          });
        
        }
      });

      /*
      console.log('booking date',bk.bookingDate);
      console.log('next shift',this.nextShift);
      console.log('cutoff',this.overCutoff);
      console.log('minbooking date',this.minBookingDate);
      console.log('slot booking',bk.startTime);
      console.log('min date',this.minDate);
      console.log('user type',this.user.userType);
      */

      if(bk.startTime === null){
        this.minDate = this.nextShift;        
      }
      else{
        this.minDate = bk.startTime;
      }

      // if(this.minDate > bk.bookingDate && (this.user.userType == 'SUP' || this.user.userType=='SUPTRAN' || this.user.userType == 'BH')){
      if(this.minDate <= this.nextShift && (this.user.userType == 'SUP' || this.user.userType=='SUPTRAN' || this.user.userType == 'BH')){
        this.isCompleted = false;
        this.allowToEdit = false;
        
        this.msgs.push({
          severity: 'error',
          summary: 'Error',
          detail: `Can't change booking data. \r\n ไม่สามารถแก้ไขข้อมูลได้เนื่องจากเลยช่วงเวลาในการแก้ไขแล้ว`,
          //life: 10000
        });        
      }

      if(bk.dockDoor === '' || bk.dockDoor === undefined || bk.dockDoor === null){
        this.isCompleted = false;        
        this.msgs.push({
          severity: 'info',
          summary: 'Warning',
          detail: 'Please confirm slot time booking. \r\n กรุณายืนยันช่วงเวลาในการส่งสินค้าอีกครั้ง เมื่อมีการแก้ไขข้อมูลวันที่หรือจำนวน PO',
       //   life: 10000
        });
        
        // this.messageService.add({
        //   severity: 'warning',
        //   summary: 'Warning',
        //   detail: `Please confirm slot time booking. \r\n กรุณายืนยันช่วงเวลาในการส่งสินค้าอีกครั้ง เมื่อมีการแก้ไขข้อมูลวันที่หรือจำนวน PO`,
        //   life: 10000
        // });
      }

    });
    
    this.menuItems$ = this.getMenuItems();
  }

  canApproved(){
    const dcDelayUserType: string[] = ["ADMIN","CONTROL","WAIVE","DEVELOP"];
    const bhUserType: string[] = ["ADMIN","CONTROL","TRANSPORT","BH","DEVELOP"];
    const approveUserType: string[] = ["ADMIN","CONTROL","DEVELOP","SUPERUSER"];

    this.isBh = false;
    this.isDcDelay = false;

    if(approveUserType.indexOf(this.user.userType.toUpperCase()) >= 0){
      this.isApproved = true;
    }else{
      this.isApproved = false;
    }  

    if(dcDelayUserType.indexOf(this.user.userType.toUpperCase()) >= 0){
      this.isDcDelay = true;
    }
    if(bhUserType.indexOf(this.user.userType.toUpperCase()) >= 0){
      this.isBh = true;
    }  
  }

  canDelete(item: any): boolean{
    var status: string[] = ["NEW","EDIT","OVERCAP","OVERCUTOFF","APPROVED","INTRANSIT"];
    
    if(status.indexOf(item.status.toUpperCase()) >= 0){
      return false;
    }else{      
      return true;
    }
  }

  deleteAll(){
    // validate completed booking
    var status: string[] = ["NEW","EDIT","OVERCAP","OVERCUTOFF","APPROVED","INTRANSIT"];
    this.canDeleteAll = false;

    this.bookingCreates.forEach(bookCreate => {
      if(status.indexOf(bookCreate.status.toUpperCase()) >= 0){
        if(this.canDeleteAll == true){

        }
        else{
          this.canDeleteAll = false;
        }
      }
      else{
        this.canDeleteAll = true;
      }
    });

    this.menuItems$ = this.getMenuItems();
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

          this.messageService.add({severity:'success', summary: 'Successful', detail: 'Truck Deleted', life: 10000});

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

    let whse = this.warehouse.find(x=>x.warehouseCode == whseCode);
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
    
    this.updateMerchtype(whse,whseCreate);
    this.canSaveBooking();
  }

  onSaveBooking():void{
    this.isLoading = true;
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
      bookHdr.merchType = booking.merchType;
      bookHdr.firstBookingStart = new Date (booking.startTime);      
      bookHdr.firstBookingEnd = new Date (booking.endTime);
      bookHdr.bookingStart = new Date(booking.startTime);
      bookHdr.bookingEnd = new Date(booking.endTime);
      bookHdr.contactName = this.contactName;
      bookHdr.contactEmail = this.contactEmail;
      bookHdr.contactTel = this.contactPhone;
      bookHdr.bookingDetails = [] as BookingDetail[];
      bookHdr.bookingTrucks = [] as BookingTruck[];
      bookHdr.postPoned = booking.postponed;
      bookHdr.backHaul = booking.backHaul;
      bookHdr.remarkDelay = booking.remarkDelay;
      bookHdr.isDelay = booking.isDelay ? 1 : 0;
      bookHdr.remark = booking.remark;

      let totalQty = 0;
      booking.bookingDetail.forEach(bookingDtl => {
        let bookDtl = {} as BookingDetail;
        bookDtl = bookingDtl;
        bookDtl.postponed = bookingDtl.isPostPoned ? 'Y' : 'N';
        bookDtl.isDelay = bookDtl.alert === 'Y' ? true : false;
        bookDtl.delayReason = bookDtl.isDelay ? bookingDtl.isPostPoned ? 'DC Delay' : 'Not Receive On-Time' : '';
        bookHdr.bookingDetails.push(bookDtl);
        totalQty += bookingDtl.weight;
      });

      booking.bookingTruck.forEach(bookingTruck => {
        let bookTruck = {} as BookingTruck;
        bookTruck = bookingTruck;
        bookHdr.bookingTrucks.push(bookTruck);
      });

      bookHdr.totalQty = totalQty;

      this.bookingKeyDto.bookingHeaders.push(bookHdr);
    });

    this.bookingKeyService
        .update(this.bookingKeyDto)
        .pipe(untilDestroyed(this))
        .subscribe({ next : (result) => { 
          this.messageService.add({severity:'success', summary: 'Update Booking completed', detail: ""});     
          this.isLoading = false;
          
          if(result[0] === 'DELETED'){            
            this.router.navigate(['/']);
          }          
          else{
            this.ngOnInit();
          }
        },
        error: (error)=> this.handleErrors(error)});
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
    //this.isLoading = true;
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete Booking for all warehouse?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.bookingHeaderService
        .deleteall(this.bookingCreates[0].internalHeaderKey)
        .pipe(untilDestroyed(this))
        .subscribe({next : (result) => { 
          this.messageService.add({severity:'success', summary: 'Delete Booking completed', detail: ""});     
          this.isLoading = false;
          this._location.back();
        },
        error:(error) => this.handleErrors(error)});
      },
      reject: () => {
        this.isLoading = false;
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
        this.bookingCreates = this.bookingCreates.filter(x=>x.bookingId !== bookingDel.bookingId);      
        this.messageService.add({severity:'success', summary: 'Delete Booking completed', detail: ""});       
        this.canSaveBooking();        
      }
    });   
  }

  onApproved(bookingAp: BookingCreate): void{
    this.isLoading = true;
    this.confirmationService.confirm({
      message: 'Are you sure you want to approved this Booking?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.bookingHeaderService
        .approved(bookingAp.internalHeaderKey)
        .pipe(untilDestroyed(this))
        .subscribe({next : (result) => {
          this.messageService.add({severity:'success', summary: 'Booking approved', detail: ""});
          this.isLoading = false;          
          this.ngOnInit();
        },
        error: (error)=>this.handleErrors(error)});    
      },
      reject: () => {
        this.isLoading = false;
      }
    });
  }
  
  onBack() {    
    this._location.back();
  }

  getPo(item: any) {
    this.ref = this.dialogService.open(PoDialogComponent, {
        header: 'Choose a PO',
        width: '90vw',
        height: '90vh',
        contentStyle: {"height": "90%", "overflow": "auto"},
        baseZIndex: 10000,
        data: {
          warehouseCode: item.warehouseCode,
          bookingDate: this.bookingDate,
          supplierCode: this.supplierSelected,
          supplierName: this.supplierName,          
          internalSupGroupId: this.internalSupGroupId,
          selectedPoList: this.selectedPoList
        }
    });

    this.ref.onClose.subscribe((ret: any) =>{
        if (ret)
        {          
          ret.forEach(po => {            
            this.addPoData(po.po_Nbr,item);           
            this.selectedPoList.push(po.po_Nbr); 
          });
        }

    });
  }

  onBackhaulChange(data: any){    
    this.canSaveBooking();
  }

  onPostponedChange(data: any){
    this.bookingCreates.find(x=>x.bookingDetail.find(x=>x.internalDetailKey == data.internalDetailKey).internalDetailKey > 0).allApproved = false;    
    this.canSaveBooking();
  }

  onAllDcDelayClick(data: any){    
    this.bookingCreates.forEach((bc) => {
      
      if(bc.allApproved.toString() === 'true'){        
        bc.bookingDetail.forEach((bd)=>{
          bd.isPostPoned = true;
        });
      }else{
        bc.bookingDetail.forEach((bd)=>{               
          bd.isPostPoned = false;
        });
      }
    });
  }

  backhaulSave(data: any){
    let backHaul : BackhaulBooking;
    backHaul = {} as BackhaulBooking;    
    backHaul.bookingHeaderId = data.internalHeaderKey;
    backHaul.isBackhaul = data.backHaul;
    this.bookingHeaderService.backhaulBooking(backHaul)
    .pipe(untilDestroyed(this))
    .subscribe({next : (result) => {
      this.messageService.add({severity:'success', summary: 'Update backhaul success', detail: ""});
      this.isLoading = false;
      this.ngOnInit();
    },
    error: (error)=>this.handleErrors(error)});    
  }

  dcDelaySave(data: any){    
    this.isLoading = true;
    let dcDelay : DcDelayBooking;
    dcDelay = {} as DcDelayBooking;
    dcDelay.bookingHeaderId = data.internalHeaderKey;
    dcDelay.bookingDetails = [];
    data.bookingDetail.forEach((bd)=>{
      if (bd.isPostPoned && bd.isDelay){
        bd.delayReason = "DC Delay";
      }
      if (!bd.isPostPoned && bd.isDelay) {
        bd.delayReason = "Not Receive On-Time";
      }
      dcDelay.bookingDetails.push(bd);
    });

    this.bookingHeaderService.dcDelayBooking(dcDelay)
    .pipe(untilDestroyed(this))
    .subscribe({next : (result) => {
      this.messageService.add({severity:'success', summary: 'Update DC Delay success', detail: ""});
      this.isLoading = false;
      this.ngOnInit();
    },
    error: (error)=>this.handleErrors(error)});    
  }

  onBookingDateChange(event: any){
    // re calculate delay
    this.bookingKeyDto.bookingHeaders.forEach((bk)=>{
      bk.dockDoor = null;
      bk.bookingDetails.forEach((bd)=>{
        bd.planRec = new Date(bd.planRec);        
        if(bd.planRec < event){
          bd.alert = 'Y';
          bd.isDelay = true;          
        }
      });
    });
    
    this.bookingCreates.forEach((bc) => {
      bc.dockDoor = '';
      bc.bookingDetail.forEach((bd)=>{        
        bd.planRec = new Date(bd.planRec);
        if(bd.planRec < event){
          bd.alert = 'Y';
          bd.isDelay = true;          
        }
      });
    });

    this.canSaveBooking();

    //this.bookingDate = event;
    //this.ngOnInit();
  }

  onCheckCancel(item): boolean {
    const cancelStatus: string[] = ["NEW","EDIT","APPROVED","OVERCAP","OVERCUTOFF","INTRANSIT"];
    const userTypes: string[] = ["ADMIN","SUPERUSER","CONTROL","DEVELOP"];

    if(userTypes.includes(this.user.userType.toUpperCase())){
      return cancelStatus.includes(item.status) ? false : true;
    }else{
      return true;
    }

    
  }

  onCancel(item: any){    
    this.showRemark = true;
    this.remarkCancel = '';
    this.cancelBookingHdr = {} as BookingHeaderDto;
    this.cancelBookingHdr.bookingId = item.bookingId;
    this.cancelBookingHdr.supCode = '';
    this.cancelBookingHdr.supName = '';
    this.cancelBookingHdr.contactTel = '';
    this.cancelBookingHdr.contactName = '';
    this.cancelBookingHdr.contactEmail = '';
    this.cancelBookingHdr.bookingTrucks = [] as BookingTruck[];
    this.cancelBookingHdr.warehouseCode = item.warehouseCode;
    this.cancelBookingHdr.bookingDetails = [] as BookingDetail[];
  }

  onSaveCancel(){
    this.confirmationService.confirm({
      message: 'Are you sure you want to cancel this Booking?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.cancelBookingHdr.remarkCancel = this.remarkCancel;
        this.bookingHeaderService.cancelBooking(this.cancelBookingHdr)
        .pipe(untilDestroyed(this))
        .subscribe({next : (result) => {
          this.messageService.add({severity:'success', summary: 'Cancel Booking success', detail: ""});
          this.isLoading = false;
          this.showRemark = false;
          this.ngOnInit();
        },
        error: (error)=>this.handleErrors(error)});
      }
    });   
  }

}
