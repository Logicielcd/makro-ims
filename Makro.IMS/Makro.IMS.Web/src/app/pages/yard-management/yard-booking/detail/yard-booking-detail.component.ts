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
import { Supplier, SupplierGroup } from '@core/models/master/supplier.model';
import { PoList } from '@core/models/booking/po.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { SlotTimeDtlComponent } from './dialog/slot-time-dtl.component';
import { User } from '@core/models/account/user.model';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { EstTime } from '@core/models/master/est-time.model';
import { Door } from '@core/models/master/door.model';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { Observable, catchError, combineLatest, concat, concatMap, map, of, switchMap } from 'rxjs';
import { SlotTimeModalComponent } from '@theme/components/modal/slot-time-modal/slot-time-modal.component';
import { TruckCap } from '@core/models/master/truck-cap.model';
import { Operation } from '@core/models/master/operation.model';
import { OperationService } from '@core/services/master/operation.service';
import { environment } from '@environments/environment';
import { BackhaulBooking } from '@core/models/booking/backhaul-booking';
import { DcDelayBooking } from '@core/models/booking/dcdelay-booking';

@UntilDestroy()
@Component({
  selector: 'app-yard-booking-detail',
  templateUrl: './yard-booking-detail.component.html',
})
export class YardBookingDetailComponent implements OnInit {

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
    private _location: Location,    
    private operationService: OperationService
  ) {

    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.warehousecapacity)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));

    this.canModify = true;
    this.canDeleteAll = false;

    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
      
    this.menuItems$ = this.getMenuItems();
    
   // this.bookingDate.setDate(this.bookingDate.getDate() + 1);
    this.user = this.authService.getUser();
    if(this.user.userType.toLowerCase() !== 'sup'){
      this.isAdmin = true;
    }
    else{
      this.isAdmin = false;
    }

    this.minDate = new Date();
    this.minDate.setDate(this.minDate.getDate() - 1);

  }

  getMenuItems(): Observable<MenuItem[]> {
    const menus: MenuItem[] = [      
      {
        label: 'Save booing',
        icon: 'pi pi-save',
        disabled: !this.isCompleted,
        command: () => {
            this.onSaveBooking();
        }
      },
      {
          label: 'Delete all booking',
          icon: 'pi pi-trash',
          disabled: this.canDeleteAll,
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
  }

  initializeColumns() {
    this.cols = this.getCommonColumns([          
        { field: 'totalQty', display: 'string', header: 'TotalQty' },
        { field: 'fullPl', display: 'string', header: 'Full' },
        { field: 'con', display: 'string', header: 'Con' },
        { field: 'non', display: 'string', header: 'Non' },
        { field: 'weight', display: 'string', header: 'Weight' }
    ]);

    this.colsCs = this.getCommonColumns([          
        { field: 'totalQty', display: 'string', header: 'TotalQty' },
        { field: 'fullPl', display: 'string', header: 'Full' },
        { field: 'fullCs', display: 'string', header: 'Full CS' },
        { field: 'halfPl', display: 'string', header: 'Half' },
        { field: 'halfCs', display: 'string', header: 'Half CS' },
        { field: 'con', display: 'string', header: 'Con' },
        { field: 'non', display: 'string', header: 'Non' },
        { field: 'weight', display: 'string', header: 'Case' }
    ]);

    this.truckCols = [
        { field: 'truckName', display: 'string', header: 'Truck Type' },
        { field: 'totalTruck', display: 'string', header: 'Total Truck' }
    ];
  }

  getCommonColumns(additionalColumns: any[]) {
      const baseColumns = [
          { field: 'alert', display: 'icon', header: 'Alert' },
          { field: 'merchType', display: 'string', header: 'Operation Type' },
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
      this.operationService.getAll()
    ])
    .pipe(
      untilDestroyed(this),
      switchMap(([whseData, truckData, truckCapData, operationData]) => {
        this.warehouse = whseData.map(x => ({
          ...x,
          warehouseDisplay: `${x.warehouseCode}-${x.warehouseName}`
        }));
        this.trucks = truckData;
        this.truckCaps = truckCapData;
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

    this.bookingKeyDto.bookingHeaders.forEach(hdr => {
      if (!['NEW', 'INTRANSIT', 'APPROVED'].includes(hdr.status.toUpperCase())) {
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

      hdr.bookingDetails.forEach(dtl => {
        dtl.alert = dtl.isDelay ? 'Y' : 'N';
        dtl.delayReason = dtl.delayReason;
        dtl.expireDate = new Date(dtl.expireDate);
        dtl.isPostPoned = dtl.delayReason === 'DC Delay' ? true : false;

        bkHeader.bookingDetails.push(dtl);        
      });

      this.bookingKey.bookingHeaders.push(bkHeader);

      const whse = this.warehouse.find(x => x.warehouseCode === hdr.warehouseCode);
      if (whse) {
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
          bookingDetail: [...hdr.bookingDetails],
          bookingTruck: hdr.bookingTrucks.map(truck => {
            const truckInfo = this.trucks.find(x => x.internalTruckId === truck.internalTruckId);
            return { ...truck, truckName: truckInfo?.truckName };
          }),
          allApproved: false,
          isLate: hdr.bookingDetails.find(x=>x.delayReason !== null || x.delayReason !== undefined || x.delayReason.length > 0) ? true : false,
        });
        this.bookingCreates.push(bookingCreate);
      }
    });
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
  }
  
  addPo(){    
    this.addPoData(this.inputPoNo);
  }

  addPoData(po: string){
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
        
        let minBookingDate = new Date();
        
        // คำนวณวันที่สามารถ booking ได้
        if(whse.advanceBookingDay > 0){
          if(whse.firstTimeOfDay > whse.endTimeOfDay){
            if(minBookingDate.getHours() >= whse.firstTimeOfDay){
              minBookingDate.setDate(minBookingDate.getDate() + 2 + whse.advanceBookingDay);
            }else{
              minBookingDate.setDate(minBookingDate.getDate() + 1 + whse.advanceBookingDay);
            }
          }
          else{
            minBookingDate.setDate(minBookingDate.getDate() + whse.advanceBookingDay);
          }
        }
        else{
          minBookingDate.setMinutes(minBookingDate.getMinutes() + whse.advanceBookingPeriod);
        }

        if (this.bookingKey.bookingHeaders && this.bookingKey.bookingHeaders.length > 0) {
          if (whse) {
            if (whse.warehouseLevel === 'D') {
              bh = this.bookingKey.bookingHeaders.find(x => x.warehouseCode === this.poDtl.warehouse_Code && x.merchType === this.poDtl.merch_Type);
            } else if (whse.warehouseLevel === 'C') {
              bh = this.bookingKey.bookingHeaders.find(x => x.warehouseCode === this.poDtl.warehouse_Code && x.companyCode === this.poDtl.company_Code);
            } else if (whse.warehouseLevel === 'W') {
              bh = this.bookingKey.bookingHeaders.find(x => x.warehouseCode === this.poDtl.warehouse_Code);
            }
          }
        }

        // ถ้าไม่เจอ BookingHeader สร้างใหม่
        if (!bh) {
          bh = {} as BookingHeader;
          Object.assign(bh, {
            warehouseCode: this.poDtl.warehouse_Code,
            supCode: this.supplierSelected,
            supName: this.supplierName,
            internalSupGroupId: this.internalSupGroupId,
            companyCode: this.poDtl.company_Code,
            merchType: this.poDtl.merch_Type,
            bookingDetails: [] as BookingDetail[],
            bookingTrucks: [] as BookingTruck[],
            postPoned: false,
            backHaul: false
          });

          //let whse = this.warehouse.find(x => x.warehouseCode === this.poDtl.warehouse_Code);
          if (whse) {
            let bookingCreate = {} as BookingCreate;
            Object.assign(bookingCreate, {
              warehouseCode: whse.warehouseCode,
              warehouseName: whse.warehouseName,
              warehouseDisplay: whse.warehouseLevel === 'D' ? `${whse.warehouseDisplay}-${this.poDtl.merch_Type}` : whse.warehouseDisplay,
              bookingDate: this.bookingDate,
              companyCode: this.poDtl.company_Code,
              merchType: this.poDtl.merch_Type,
              bookingDetail: [] as BookingDetail[],
              bookingTruck: [] as BookingTruck[],
              whseCap: whse.capUom,
              postponed: false,
              backHaul: false,
              isCanBooking: new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()) 
              > new Date(minBookingDate.getFullYear(),minBookingDate.getMonth(),minBookingDate.getDate())              
              ? true : false,
              minBookingDateTime: minBookingDate,
              isLate: new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()) 
              > new Date(this.poDtl.plan_Receive_Date.getFullYear(),this.poDtl.plan_Receive_Date.getMonth(),this.poDtl.plan_Receive_Date.getDate())              
              ? true : false,
              allApproved: false,
            });

            this.bookingCreates.push(bookingCreate);
            this.bookingKey.bookingHeaders.push(bh);
          }
        }
        
        this.addBookingDetail(this.poDtl);
        this.selectedPoList.push(this.inputPoNo);
        this.inputPoNo = '';
        this.isLoading = false;
      },
      error: (error) => this.handleErrors(error)
    });
  }

  addBookingDetail(po: PoList): void {
    // หา warehouse ที่ตรงกับ po.warehouse_Code
    const whse = this.warehouse.find(x => x.warehouseCode === po.warehouse_Code);
    if (!whse) return; // ถ้าไม่เจอ warehouse ก็หยุดทำงาน
  
    // หา bookingHeader ที่ตรงกับ warehouseLevel และข้อมูลที่เกี่ยวข้อง
    let header = this.bookingKey.bookingHeaders.find(x =>
      x.warehouseCode === po.warehouse_Code &&
      (whse.warehouseLevel === 'D' ? x.merchType === po.merch_Type :
       whse.warehouseLevel === 'C' ? x.companyCode === po.company_Code : true)
    );
  
    // ตรวจสอบว่ามี po.Nbr ที่ตรงกันใน bookingDetails หรือไม่
    let dtl = header?.bookingDetails.find(x => x.poNbr === po.po_Nbr);

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
        remark: po.deplay_Reason,
        isPostPoned: po.postPoned === 'N' ? false : true
      });
  
      // เพิ่มข้อมูลใน bookingHeaders และ bookingCreates
      header?.bookingDetails.push(dtl);
      
      let bookingCreate = this.bookingCreates.find(x =>
        x.warehouseCode === po.warehouse_Code &&
        (whse.warehouseLevel === 'D' ? x.merchType === po.merch_Type :
         whse.warehouseLevel === 'C' ? x.companyCode === po.company_Code : true)
      );
  
      bookingCreate?.bookingDetail.push(dtl);
     
      this.updateMerchtype(whse,bookingCreate);
      this.updateDelayDelivery(whse,bookingCreate);
    } else {
      // ถ้าซ้ำไม่ทำอะไร
      //console.log('duplicate po detail');
    }
  }
  
  updateDelayDelivery(whse:any,bkCreate: BookingCreate){

    let minBookingDate = new Date();
        
    // คำนวณวันที่สามารถ booking ได้
    if(whse.advanceBookingDay > 0){
      if(whse.firstTimeOfDay > whse.endTimeOfDay){
        if(minBookingDate.getHours() >= whse.firstTimeOfDay){
          minBookingDate.setDate(minBookingDate.getDate() + 2 + whse.advanceBookingDay);
        }else{
          minBookingDate.setDate(minBookingDate.getDate() + 1 + whse.advanceBookingDay);
        }
      }
      else{
        minBookingDate.setDate(minBookingDate.getDate() + whse.advanceBookingDay);
      }
    }
    else{
      minBookingDate.setMinutes(minBookingDate.getMinutes() + whse.advanceBookingPeriod);
    }

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
    
  }

  updateMerchtype(whse:any, bkCreate: BookingCreate){
    if (whse.capUom == 'CS'){
      let totalPl = 0;
      let totalCon = 0;
      let totalNon = 0;
      bkCreate?.bookingDetail.forEach((dtl)=> {
        totalPl += dtl.fullPl + (dtl.halfPl / 2);
        totalCon += dtl.con;
        totalNon += dtl.non;
      });

      if(bkCreate.truckSelected !== undefined && bkCreate.truckSelected !== 0){
        let truck = this.trucks.find(x=>x.internalTruckId == bkCreate.truckSelected);
        
        if(truck !== undefined && truck !== null && truck.truckGroup !== null && truck.truckGroup !== undefined && truck.truckGroup.trim() !== ''){
          bkCreate.merchType = truck.truckGroup;
          bkCreate.merchTypeDisplay = this.operationTypes.find(x=>x.warehouseCode == bkCreate.warehouseCode && x.operationName == truck.truckGroup).description;
        }
        else{
          
          let truckCap = this.truckCaps.find(x=>x.internalTruckId == bkCreate.truckSelected && x.warehouseCode == bkCreate.warehouseCode);

          if(totalPl >= truckCap.fullPl){           
            bkCreate.merchType = truckCap.operationType;
            bkCreate.merchTypeDisplay = this.operationTypes.find(x=>x.warehouseCode == bkCreate.warehouseCode && x.operationName == truckCap.operationType).description;
          }else if(totalCon > totalNon){
            bkCreate.merchType = "CON";
            bkCreate.merchTypeDisplay = this.operationTypes.find(x=>x.warehouseCode == bkCreate.warehouseCode && x.operationName == 'CON').description;
          }
          else{            
            bkCreate.merchType = "NON-CON";
            bkCreate.merchTypeDisplay = this.operationTypes.find(x=>x.warehouseCode == bkCreate.warehouseCode && x.operationName == 'NON-CON').description;
          }

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

  deletePo(delPo:any){
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected PO?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
          this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr)).bookingDetail = 
          this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr)).bookingDetail.filter(x=>x.poNbr != delPo.poNbr);
          this.messageService.add({severity:'success', summary: 'Successful', detail: 'PO Deleted', life: 10000});
          
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

  show(whseCode: string, companyCode: string, merchType: string) {
    this.totalEstTime = 0;
  
    const whse = this.warehouse.find(x => x.warehouseCode === whseCode);
    const booking = this.getBooking(whseCode, companyCode, merchType);
    const bookingId = booking?.bookingId;
    const bookingHdrKeyId = booking?.internalHeaderKey;
  
    if (whse) {
      this.calculateEstTime(whse, merchType, companyCode);
    }
  
    const modalComponent =  SlotTimeModalComponent;
  
    this.ref = this.dialogService.open(modalComponent, {
      header: 'Choose a time slot',
      width: '70%',
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
        internalSupGroupId: this.internalSupGroupId
      }
    });
  
    this.ref.onClose.subscribe((ret: any) => {
      if (ret) {
        this.handleModalClose(ret, whse, whseCode, companyCode, merchType);
      }
    });
  }
  
  getBooking(whseCode: string, companyCode: string, merchType: string) {
    return this.bookingCreates.find(x =>
      x.warehouseCode === whseCode &&
      (x.merchType === merchType || x.companyCode === companyCode || x.warehouseCode === whseCode)
    );
  }
  
  calculateEstTime(whse: any, merchType: string, companyCode: string) {
    const relevantBooking = this.bookingCreates.find(x =>
      x.warehouseCode === whse.warehouseCode &&
      (whse.warehouseLevel === 'D' ? x.merchType === merchType :
        whse.warehouseLevel === 'C' ? x.companyCode === companyCode :
          true)
    );
  
    relevantBooking?.bookingTruck.forEach(truck => {
      const estTime = this.estTimes.find(x => x.internalTruckId == truck.internalTruckId && x.warehouseCode == whse.warehouseCode
        && x.operationType.split('|').includes(merchType)
      );
     // console.log('cal est:',estTime);
      if (estTime) {
          this.totalEstTime += estTime.hourEst * 60 + estTime.minEst;
      } else {
          this.totalEstTime = 0; // Reset to 0 if estTime not found
      }
    });

  }
  
  handleModalClose(ret: any, whse: any, whseCode: string, companyCode: string, merchType: string) {
    let doorId = 0;
    let doorName = '';
    
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
    const doorName = door?.doorName || '';
  
    this.messageService.add({
      severity: 'success',
      summary: 'Slot time selected',
      detail: doorName || ''
    });
  
    const levelCheck = (x: any) => {
      if (whse.warehouseLevel === 'D') return x.merchType === merchType;
      if (whse.warehouseLevel === 'C') return x.companyCode === companyCode;
      return true;
    };
  
    const booking = this.bookingCreates.find(x => x.warehouseCode === whseCode && levelCheck(x));
    const bookingHeader = this.bookingKeyDto.bookingHeaders.find(x => x.warehouseCode === whseCode && levelCheck(x));
  
    if (booking && bookingHeader) {
      booking.internalDoorId = doorId;
      booking.dockDoor = doorName;
      booking.startTime = new Date(ret.StartTime);
      booking.endTime = new Date(ret.EndTime);
  
      bookingHeader.internalDoorId = doorId;
      bookingHeader.dockDoor = doorName;
      bookingHeader.bookingStart = new Date(ret.StartTime);
      bookingHeader.bookingEnd = new Date(ret.EndTime);
    }
  
    this.canSaveBooking();
    this.deleteAll();
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
     
     this.menuItems$ = this.getMenuItems();
  }

  canApproved(){
    const dcDelayUserType: string[] = ["ADMIN","CONTROL"];
    const bhUserType: string[] = ["ADMIN","CONTROL","TRANSPORT"];
    if (this.bookingCreates[0].status === "NEW"){
      this.isApproved = true;
      this.isBh = false;
      this.isDcDelay = false;
    }
    else{
      this.isApproved = false;
      if(this.user.userType === "SUP"){
        this.isBh = false;
        this.isDcDelay = false;
      } 

      if(dcDelayUserType.indexOf(this.user.userType) >= 0){
        this.isDcDelay = true;
      }
      if(bhUserType.indexOf(this.user.userType) >= 0){
        this.isBh = true;
      }  
     }
  }

  canDelete(item: any): boolean{
    var status: string[] = ["NEW","APPROVED"];
    
    if(status.indexOf(item.status.toUpperCase()) >= 0){
      return false;
    }else{      
      return true;
    }
  }

  deleteAll(){
    // validate completed booking
    var status: string[] = ["NEW","APPROVED"];
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

      booking.bookingDetail.forEach(bookingDtl => {
        let bookDtl = {} as BookingDetail;
        bookDtl = bookingDtl;
        bookDtl.postponed = bookingDtl.isPostPoned ? 'Y' : 'N';
        bookDtl.isDelay = bookDtl.alert === 'Y' ? true : false;
        bookDtl.delayReason = bookDtl.isDelay ? bookingDtl.isPostPoned ? 'DC Delay' : 'Not Receive On-Time' : '';
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
        .subscribe({ next : (result) => { 
          this.messageService.add({severity:'success', summary: 'Update Booking completed', detail: ""});     
          this.isLoading = false;
          this.ngOnInit();                                            
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
    this.isLoading = true;
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
        this.messageService.add({severity:'success', summary: 'Delete Booking completed', detail: ""});       
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
      }
    });
  }
  
  onBack() {    
    this._location.back();
  }

  getPo() {    
    this.ref = this.dialogService.open(PoDialogComponent, {
        header: 'Choose a PO',
        width: '90%',
        contentStyle: {"max-height": "1000px", "overflow": "auto"},
        baseZIndex: 10000,
        data: {
          warehouseCode: "",
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
            this.addPoData(po.po_Nbr);           
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

}
