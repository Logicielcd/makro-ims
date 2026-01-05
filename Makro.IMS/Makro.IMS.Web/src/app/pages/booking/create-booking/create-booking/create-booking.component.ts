import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, MenuItem, Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';

/* service */
import { BookingKeyService } from '@core/services/booking/booking-key.service';
import { PoService } from '@core/services/booking/po.service';
import { DoorService } from '@core/services/master/door.service';
import { EstTimeService } from '@core/services/master/est-time.service';
import { SupplierService } from '@core/services/master/supplier.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { WarehouseService } from '@core/services/master/warehouse.service';

/* model */
import { User } from '@core/models/account/user.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { BookingDetail, BookingHeader, BookingHeaderDto, BookingKey, BookingKeyDto, BookingTruck } from '@core/models/booking/booking-header.model';
import { PoList } from '@core/models/booking/po.model';
import { Door } from '@core/models/master/door.model';
import { EstTime } from '@core/models/master/est-time.model';
import { Supplier, SupplierGroup } from '@core/models/master/supplier.model';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Warehouse } from '@core/models/master/warehouse.model';

import { SlotTimeModalComponent } from '@theme/components/modal/slot-time-modal/slot-time-modal.component';
import { PoDialogComponent } from '@theme/components/modal/po-dialog/po-dialog.component';

import { environment } from '@environments/environment';
import { combineLatest } from 'rxjs';
import { TruckCap, TruckRule } from '@core/models/master/truck-cap.model';
import { OperationService } from '@core/services/master/operation.service';
import { Operation } from '@core/models/master/operation.model';

@UntilDestroy()
@Component({
  templateUrl: './create-booking.component.html',
  styleUrls: ['create-booking.component.scss'],
})
export class CreateBookingComponent implements OnInit {
  title = 'Create Booking';
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

  // supplier variable
  suppliers: Supplier[];
  supplierSelected: string;

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

  // po variable
  poDtl: PoList;
  inputPoNo: string;
  cols: any[];
  colsCs: any[];
  
  selectedPoList: string[];

  // truck variable
  bookingTruck: BookingTruck;
  trucks: TruckMaster[];
  truckSelected: number;
  truckDriver: string;
  truckLicense: string;
  truckCols: any[];
  truckCaps: TruckCap[];
  truckRules: TruckRule[];

  operationTypes: Operation[];
  // estTime
  estTimes: EstTime[];
  totalEstTime: number;

  totalWeight: number;
  minDate: Date;
  msgBooking: string;

  @ViewChild('f') f: NgForm;

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    private warehouseService: WarehouseService,    
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
    private operationService: OperationService
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

    this.bookingDate = new Date();
    this.user = this.authService.getUser();
    this.minDate = new Date();
  }

  ngOnInit() {
    this.initializeVariables();
    this.fetchData();
    this.initializeColumns();
  }

  initializeVariables() {
      this.isCompleted = false;
      this.isSelect = false;
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
    combineLatest([
      this.warehouseService.getAll(),
      this.truckMasterService.getAll(),
      this.truckMasterService.getTruckCapAll(),
      this.truckMasterService.getTruckRuleAll(),
      this.operationService.getAll(),      
    ])
    .pipe(untilDestroyed(this))
    .subscribe(([whseData,truckData,truckCapData,truckRuleData,operationData]) => {
      this.warehouse = whseData.map(x=>({
        ...x,
        warehouseDisplay: `${x.warehouseCode}-${x.warehouseName}`
      }));
      this.trucks = truckData;
      this.truckCaps = truckCapData;
      this.truckRules = truckRuleData;
      this.operationTypes = operationData;
    },
    (error) => this.handleError(error)
    );

    // supplier service call
    const supplierObservable = this.user.userType.toUpperCase() !== "SUP" && this.user.userType.toUpperCase() !== "SUPTRAN"
      ? this.supplierService.getAll()
      : this.supplierService.getSupByUser(this.user.userId);

    supplierObservable
    .pipe(untilDestroyed(this))
    .subscribe(
      (data) => {
        this.suppliers = data;
        this.getSupplierValue();
      },
      (error) => this.handleError(error)
    );
  }

  handleError(error: any) {
    this.isLoading = false;
    this.msgs = [];
    if (error.Messages && Array.isArray(error.Messages)) {
      error.Messages.forEach((msg: any) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: msg, life: 10000 });
      });
    } else {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'An unexpected error occurred', life: 10000 });
    }
  }

  addPo() {
    if(this.inputPoNo.includes (',')){
      var poList = this.inputPoNo.split(',');
      poList.forEach(po => {
        this.addPoData(po.trim());
      });
    }
    else{
      this.addPoData(this.inputPoNo.trim());
    }
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
        let overCutoff: boolean = false;
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
        let nextShift = new Date();
        let curDate : Date = new Date();

        // คำนวณวันที่สามารถ booking ได้
        if(whse.advanceBookingDay > -1){
          if(whse.firstTimeOfDay > whse.endTimeOfDay){
            if (nextShift.getHours() >= whse.firstTimeOfDay){
              nextShift.setDate(nextShift.getDate() + 1);
              nextShift.setHours(whse.firstTimeOfDay);
              nextShift.setMinutes(0);
              nextShift.setSeconds(0);
            }
            else{
              nextShift.setHours(whse.firstTimeOfDay);
              nextShift.setMinutes(0);
              nextShift.setSeconds(0);
            }
           
          }
          else{
            //  minBookingDate.setDate(minBookingDate.getDate() + whse.advanceBookingDay);

            nextShift.setDate(nextShift.getDate() + 1);
            nextShift.setHours(whse.firstTimeOfDay);
            nextShift.setMinutes(0);
            nextShift.setSeconds(0);
          }
          curDate.setHours(curDate.getHours() + whse.advanceBookingDay)
          //console.log('curDate:',curDate);
          if(curDate >= nextShift){
            overCutoff = true;
            nextShift.setDate(nextShift.getDate() + 1);
          }
        }
        else{
         // minBookingDate.setMinutes(minBookingDate.getMinutes() + whse.advanceBookingPeriod);
          nextShift.setMinutes(nextShift.getMinutes() + whse.advanceBookingPeriod);
          minBookingDate.setDate(nextShift.getDate());
        }

        minBookingDate.setDate(nextShift.getDate());
        if(whse.firstTimeOfDay > whse.endTimeOfDay){
          minBookingDate.setDate(minBookingDate.getDate() + 1);
        }
      //  minBookingDate.setDate(nextShift.getDate());

      /*#poNo
      console.log('minBookingDate:',minBookingDate);
      console.log('nextShift:',nextShift);
      console.log('curDate:',curDate);
      console.log('over cutoff',overCutoff);
        */
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

        
        // กำหนดข้อมูล supplier
        let sup = this.suppliers.find(x => x.supCode === this.supplierSelected);

        // ถ้าไม่เจอ BookingHeader สร้างใหม่
        if (!bh) {
          bh = {} as BookingHeader;
          Object.assign(bh, {
            warehouseCode: this.poDtl.warehouse_Code,
            supCode: this.supplierSelected,
            supName: sup?.supName,
            internalSupGroupId: sup?.internalSupGroupId,
            companyCode: this.poDtl.company_Code,
            merchType: this.poDtl.merch_Type,
            bookingDetails: [] as BookingDetail[],
            bookingTrucks: [] as BookingTruck[],
            postPoned: false,
            backHaul: false,
            remark: '',
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
              remark: '',
              isCanBooking: new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()) 
              > new Date(minBookingDate.getFullYear(),minBookingDate.getMonth(),minBookingDate.getDate())              
              ? true : false,
              minBookingDateTime: minBookingDate,
              isLate: new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()) 
              > new Date(this.poDtl.plan_Receive_Date.getFullYear(),this.poDtl.plan_Receive_Date.getMonth(),this.poDtl.plan_Receive_Date.getDate())              
              ? true : false,
              nextShift: nextShift,
              overCutoff: overCutoff,
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
      error: (error) => this.handleError(error)
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
        delayReason: po.delay_Reason,
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
  
  deletePo(delPo:any){
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected PO?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
          let bCreate = this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr));
          const whse = this.warehouse.find(x => x.warehouseCode === bCreate.warehouseCode);

          this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr)).bookingDetail =
          this.bookingCreates.find(x=>x.bookingDetail.find(b=>b.poNbr == delPo.poNbr)).bookingDetail.filter(x=>x.poNbr != delPo.poNbr);

          this.bookingKey.bookingHeaders.find(x=>x.bookingDetails.find(b=>b.poNbr == delPo.poNbr)).bookingDetails =
          this.bookingKey.bookingHeaders.find(x=>x.bookingDetails.find(b=>b.poNbr == delPo.poNbr)).bookingDetails.filter(x=>x.poNbr != delPo.poNbr);

          this.messageService.add({severity:'success', summary: 'Successful', detail: 'PO Deleted', life: 10000});

          this.selectedPoList.splice(this.selectedPoList.indexOf(delPo.poNbr),1);

          // remove warehouse
          this.bookingCreates = this.bookingCreates.filter(x=>x.bookingDetail.length > 0);
          this.bookingKey.bookingHeaders = this.bookingKey.bookingHeaders.filter(x=>x.bookingDetails.length > 0);

          this.updateDelayDelivery(whse,bCreate);
          this.updateMerchtype(whse,bCreate);
      }
    });
  }

  onSelectSup(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);
    if(this.contactName.length > 0 && this.contactEmail.length > 0 && this.contactPhone.length > 0){
      this.isSelect = true;
      this.estTimeService
      .getBySupGroupId(sup.internalGroupId)
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.estTimes = data;
        this.supplierService.getSupGroup(sup.internalGroupId).pipe(untilDestroyed(this)).subscribe((supGrp)=>{
          this.supplierGroup = supGrp;
          this.supplierGroup.isVip = this.supplierGroup.isVip == 'Y' ? 'Fix time slot' : '';
        });
      },(error)=>this.handleError(error));
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

  show(whseCode: string, companyCode: string, merchType: string) {
    // Reset total estimated time and weight
    this.totalEstTime = 0;
    this.totalWeight = 0;

    const whse = this.warehouse.find(x => x.warehouseCode == whseCode);
    if (!whse) return;

    const booking = this.getBooking(whse, whseCode, companyCode, merchType);
    if (booking) {
        this.calculateEstTime(whse,merchType, booking.bookingTruck);
        this.calculateWeight(booking.bookingDetail);
    }

    this.openSlotTimeModal(whse, whseCode, companyCode, merchType);
  }

  getBooking(whse: any, whseCode: string, companyCode: string, merchType: string) {
      if (whse.warehouseLevel === 'D') {
          return this.bookingCreates.find(x => x.warehouseCode == whseCode && x.merchType == merchType);
      } else if (whse.warehouseLevel === 'C') {
          return this.bookingCreates.find(x => x.warehouseCode == whseCode && x.companyCode == companyCode);
      } else if (whse.warehouseLevel === 'W') {
          return this.bookingCreates.find(x => x.warehouseCode == whseCode);
      }
      return null;
  }

  calculateEstTime(whse: any,merchType: string,bookingTrucks: any[]) {
    // get max truck type
    let maxTruck = '000';
    bookingTrucks.forEach(truck=>{
      maxTruck = maxTruck > this.trucks.find(x=>x.internalTruckId == truck.internalTruckId).sequence ? maxTruck : this.trucks.find(x=>x.internalTruckId == truck.internalTruckId).sequence;
    })

    let maxTruckId = this.trucks.find(x=>x.sequence == maxTruck).internalTruckId;

    bookingTrucks.filter(x=>x.internalTruckId == maxTruckId).forEach(truck => {
        // const estTime = this.estTimes.find(x => x.internalTruckId == truck.internalTruckId && x.warehouseCode == whse.warehouseCode
        //   && x.operationType.split('|').includes(merchType)
        // );
        // // console.log('cal est:',estTime);
        // if (estTime) {
        //     this.totalEstTime += estTime.hourEst * 60 + estTime.minEst;
        // } else {
        //     this.totalEstTime += 0; // Reset to 0 if estTime not found
        // }
        const estTime = this.estTimes.find(x => x.internalTruckId == maxTruckId && x.warehouseCode == whse.warehouseCode
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

  calculateWeight(bookingDetails: any[]) {
      bookingDetails.forEach(po => {
          this.totalWeight += po.weight;
      });
  }

  openSlotTimeModal(whse: any, whseCode: string, companyCode: string, merchType: string) {
      const modalComponent = SlotTimeModalComponent;
      this.ref = this.dialogService.open(modalComponent, {
          header: 'Choose a time slot of ' + merchType,
          width: '1280px',
          contentStyle: { "max-height": "1000px", "overflow": "auto" },
          baseZIndex: 10000,
          data: {
              warehouseCode: whseCode,
              bookingDate: this.bookingDate,
              supplierCode: this.supplierSelected,
              supplierName: this.suppliers.find(x => x.supCode == this.supplierSelected).supName,
              totalEstTime: this.totalEstTime,
              bookingSlot: this.getBooking(whse, whseCode, companyCode, merchType),
              internalSupGroupId: this.suppliers.find(x => x.supCode == this.supplierSelected).internalGroupId,
              totalWeight: this.totalWeight
              
          }
      });

      this.handleModalResponse(whse, whseCode, companyCode, merchType);
  }

  handleModalResponse(whse: any, whseCode: string, companyCode: string, merchType: string) {
      let door: Door = {} as Door;
      let doorId = 0;
      let doorName = '';
      let returnData: any;
      let status = '';
      this.ref.onClose.subscribe((ret: any) => {
        if (ret) {
          returnData = ret;
          if(ret.EventType !== null && ret.EventType !== undefined){
            this.doorService.getById(ret.EventType).pipe(untilDestroyed(this)).subscribe((data) => {
              door = data;
              doorId = door.internalDoorId;
              doorName = door.doorName;
              returnData = ret;              
          });
          }            
          this.updateBooking(whse, whseCode, doorId, doorName, returnData, companyCode, merchType);
          this.messageService.add({
              severity: 'success',
              summary: 'Slot time already selected',
              detail: doorName
          });
          this.canSaveBooking();              
        }
      });
  }

  updateBooking(whse: any, whseCode: string, doorId: number, doorName: string, returnData: any, companyCode: string, merchType: string) {
    const booking = this.getBooking(whse, whseCode, companyCode, merchType);

    console.log('return data',returnData);

    if (booking) {
      booking.internalDoorId = doorId;
      booking.dockDoor = doorName;
      booking.startTime = returnData.StartTime;
      booking.endTime = returnData.EndTime;
      booking.status = returnData.OverCap ? 'OVERCAP' : '';
      
      if(returnData.OverCutoff){
        booking.status = 'OVERCUTOFF';
      }

    }
  }

  canSaveBooking() {
    // validate completed booking
    const totalBooking = this.bookingCreates.length;
    const totalCompleted = this.bookingCreates.filter(bookCreate => 
        bookCreate.startTime && bookCreate.bookingTruck.length > 0
    ).length;

    this.isCompleted = totalBooking === totalCompleted;
  }

  deleteTruck(delTruck:any){
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected Truck?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {

        let bCreate = this.bookingCreates.find(x=>x.bookingTruck.find(b=>b.internalTruckId == delTruck.internalTruckId));
        const whse = this.warehouse.find(x => x.warehouseCode === bCreate.warehouseCode);

        this.bookingCreates.find(x=>x.bookingTruck.find(b=>b.internalTruckId == delTruck.internalTruckId)).bookingTruck =
        this.bookingCreates.find(x=>x.bookingTruck.find(b=>b.internalTruckId == delTruck.internalTruckId)).bookingTruck.filter(x=>x.internalTruckId != delTruck.internalTruckId);
        this.bookingKey.bookingHeaders.find(x=>x.bookingTrucks.find(b=>b.internalTruckId == delTruck.internalTruckId)).bookingTrucks =
        this.bookingKey.bookingHeaders.find(x=>x.bookingTrucks.find(b=>b.internalTruckId == delTruck.internalTruckId)).bookingTrucks.filter(x=>x.internalTruckId != delTruck.internalTruckId);
        this.messageService.add({severity:'success', summary: 'Successful', detail: 'Truck Deleted', life: 10000});
        
        this.updateMerchtype(whse,bCreate);
        this.updateDelayDelivery(whse,bCreate);
        
        this.canSaveBooking();
      }
    });
  }

  onTruckSelected(whseCode:string,companyCode:string,merchType:string,event: any){
    let whse = this.warehouse.find(x=>x.warehouseCode == whseCode);
    if(whse.warehouseLevel == 'D'){
      this.bookingCreates.find(x=>x.warehouseCode == whseCode && x.merchType == merchType).truckSelected = event.value;
    }
    else if(whse.warehouseLevel == 'C'){     
      this.bookingCreates.find(x=>x.warehouseCode == whseCode && x.companyCode == companyCode).truckSelected = event.value;
    }
    else if(whse.warehouseLevel == 'W'){      
      this.bookingCreates.find(x=>x.warehouseCode == whseCode).truckSelected = event.value;
    }    
  }

  addTruck(whseCode: string, companyCode: string, merchType: string) {
    const whse = this.warehouse.find(x => x.warehouseCode == whseCode);
    if (!whse) return;
    let update = false;
    let whseCreate: BookingCreate = this.getBooking(whse,whseCode, companyCode, merchType);
    if (!whseCreate) return;

    // Check if same truck type exists and update its totalTruck
    update = this.updateExistingTruck(whseCreate, whse, whseCode, companyCode, merchType);

    // If truck is not updated, add a new truck
    if (!update) {
        this.addNewTruck(whse, whseCreate, whseCode, companyCode, merchType);
    }

    this.updateMerchtype(whse,whseCreate);

    this.updateDelayDelivery(whse,whseCreate);
    // Reset totalTruck after addition
    whseCreate.totalTruck = null;

    this.canSaveBooking();
  }

  updateExistingTruck(whseCreate: BookingCreate, whse: any, whseCode: string, companyCode: string, merchType: string): boolean {
      let update = false;
      const trucks = whseCreate.bookingTruck;

      trucks.forEach(truck => {
          if (truck.internalTruckId == whseCreate.truckSelected) {
              truck.totalTruck += Number.parseInt(whseCreate.totalTruck.toString());
              const bookingTruck = this.getBookingHeaderTruck(whse,whseCode, companyCode, merchType, whseCreate.truckSelected);
              if (bookingTruck) {
                  bookingTruck.totalTruck = truck.totalTruck;
              }
              update = true;
          }
      });
      return update;
  }

  getBookingHeaderTruck(whse: any,whseCode: string, companyCode: string, merchType: string, truckId: number): any {      
      if (whse.warehouseLevel == 'D') {
          return this.bookingKey.bookingHeaders
              .find(x => x.warehouseCode == whseCode && x.merchType == merchType)
              ?.bookingTrucks.find(x => x.internalTruckId == truckId);
      } else if (whse.warehouseLevel == 'C') {
          return this.bookingKey.bookingHeaders
              .find(x => x.warehouseCode == whseCode && x.companyCode == companyCode)
              ?.bookingTrucks.find(x => x.internalTruckId == truckId);
      } else if (whse.warehouseLevel == 'W') {
          return this.bookingKey.bookingHeaders
              .find(x => x.warehouseCode == whseCode)
              ?.bookingTrucks.find(x => x.internalTruckId == truckId);
      }
      return null;
  }

  addNewTruck(whse:any, whseCreate: BookingCreate, whseCode: string, companyCode: string, merchType: string) {
    let bkTruck = {} as BookingTruck;
    bkTruck.totalTruck = whseCreate.totalTruck;
    bkTruck.internalTruckId = whseCreate.truckSelected;
    bkTruck.truckName = this.trucks.find(x => x.internalTruckId == whseCreate.truckSelected)?.truckName || '';
    
    this.getBooking(whse,whseCode, companyCode, merchType).bookingTruck.push(bkTruck);
    this.getBookingHeader(whse,whseCode, companyCode, merchType).bookingTrucks.push(bkTruck);
  }

  getBookingHeader(whse: any,whseCode: string, companyCode: string, merchType: string) {      
      if (whse.warehouseLevel == 'D') {
          return this.bookingKey.bookingHeaders.find(x => x.warehouseCode == whseCode && x.merchType == merchType);
      } else if (whse.warehouseLevel == 'C') {
          return this.bookingKey.bookingHeaders.find(x => x.warehouseCode == whseCode && x.companyCode == companyCode);
      } else if (whse.warehouseLevel == 'W') {
          return this.bookingKey.bookingHeaders.find(x => x.warehouseCode == whseCode);
      }
      return null;
  }

  updateDelayDelivery(whse:any,bkCreate: BookingCreate){
    let nextShift = new Date();
    let minBookingDate = new Date();
        
    // คำนวณวันที่สามารถ booking ได้
    if(whse.advanceBookingDay >= 0){
      if(whse.firstTimeOfDay > whse.endTimeOfDay){
        if (nextShift.getHours() >= whse.firstTimeOfDay){
          nextShift.setDate(nextShift.getDate() + 1);
          nextShift.setHours(whse.firstTimeOfDay);
          nextShift.setMinutes(0);
          nextShift.setSeconds(0);
        }
        else{
          nextShift.setHours(whse.firstTimeOfDay);
          nextShift.setMinutes(0);
          nextShift.setSeconds(0);
        }
        

        if(minBookingDate.getHours() >= whse.firstTimeOfDay){
          minBookingDate.setDate(minBookingDate.getDate() + 2 + whse.advanceBookingDay);
        }else{
          minBookingDate.setDate(minBookingDate.getDate() + 1 + whse.advanceBookingDay);
        }
      }
      else{
        nextShift.setDate(nextShift.getDate() + 1);
        nextShift.setHours(whse.firstTimeOfDay);
        nextShift.setMinutes(0);
        nextShift.setSeconds(0);

        minBookingDate.setDate(minBookingDate.getDate() + whse.advanceBookingDay);
      }
    }
    else{
      nextShift.setMinutes(nextShift.getMinutes() + whse.advanceBookingPeriod);
      minBookingDate.setMinutes(minBookingDate.getMinutes() + whse.advanceBookingPeriod);
    }

    minBookingDate = nextShift;
   
    let checkLate = false;

    let bDate = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate());
    
    bkCreate.bookingDetail.forEach(po => {
      let pDate = new Date(po.planRec.getFullYear(),po.planRec.getMonth(),po.planRec.getDate())
      if(bDate > pDate){
        checkLate = true;
      }
    });

    bkCreate.isLate = checkLate;
    
  }

  updateMerchtype(whse:any, bkCreate: BookingCreate){
 //   console.log(whse);
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
  
  onSaveBooking():void{
    this.isLoading = true;

    let bookingKeyDto: BookingKeyDto = {} as BookingKeyDto;
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);

    bookingKeyDto.bookingHeaders = [] as BookingHeaderDto[];   
    bookingKeyDto.bookingDate = this.bookingDate;
    bookingKeyDto.userName = this.user.userId;
    
    this.bookingCreates.forEach(booking => {
      let bookHdr = {} as BookingHeaderDto;
      bookHdr.warehouseCode = booking.warehouseCode;
      bookHdr.internalDoorId = booking.internalDoorId;
      bookHdr.internalSupGroupId = sup.internalGroupId;
      bookHdr.supCode = this.supplierSelected;      
      bookHdr.supName = sup.supName;
      bookHdr.firstBookingStart = new Date (booking.startTime);
      bookHdr.firstBookingEnd = new Date (booking.endTime);
      bookHdr.bookingStart = new Date(booking.startTime);
      bookHdr.bookingEnd = new Date(booking.endTime);
      bookHdr.contactName = this.contactName;
      bookHdr.contactEmail = this.contactEmail;
      bookHdr.contactTel = this.contactPhone;
      bookHdr.bookingDetails = [] as BookingDetail[];
      bookHdr.bookingTrucks = [] as BookingTruck[];
      bookHdr.companyCode = booking.companyCode;
      bookHdr.merchType = booking.merchType;
      bookHdr.postPoned = booking.postponed;
      bookHdr.backHaul = booking.backHaul;
      bookHdr.remarkDelay = booking.remarkDelay;
      bookHdr.remark = booking.remark;
      bookHdr.status = booking.status;
      // bookHdr.isDelay = booking.bookingDetail.find(x=>x.alert === 'Y').alert === 'Y' ? 1 : 0;

      booking.bookingDetail.forEach(bookingDtl => {
        let bookDtl = {} as BookingDetail;
        bookDtl = bookingDtl;        
        bookDtl.postponed = bookingDtl.isPostPoned ? 'Y' : 'N';
        bookDtl.isDelay = bookDtl.alert === 'Y' ? true : false;
        bookDtl.delayReason = bookDtl.isDelay ? bookingDtl.isPostPoned ? 'DC Delay' : 'Not Receive On-Time' : '';
        bookDtl.remark = bookingDtl.remark;
        bookHdr.bookingDetails.push(bookDtl);
      });

      booking.bookingTruck.forEach(bookingTruck => {
        let bookTruck = {} as BookingTruck;
        bookTruck = bookingTruck;
        bookHdr.bookingTrucks.push(bookTruck);
      });

      bookingKeyDto.bookingHeaders.push(bookHdr);
    });

    this.bookingKeyService
    .add(bookingKeyDto)
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
        error: (error) => this.handleError(error)
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

  getPo(whse:string) {
    this.ref = this.dialogService.open(PoDialogComponent, {
        header: 'Choose a PO',
        width: '90%',
        contentStyle: {"max-height": "1000px", "overflow": "auto"},
        baseZIndex: 10000,
        data: {
          warehouseCode: whse,
          bookingDate: this.bookingDate,
          supplierCode: this.supplierSelected,
          supplierName: this.suppliers.find(x=>x.supCode == this.supplierSelected).supName,          
          internalSupGroupId: this.suppliers.find(x=>x.supCode == this.supplierSelected).internalGroupId,
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


}
