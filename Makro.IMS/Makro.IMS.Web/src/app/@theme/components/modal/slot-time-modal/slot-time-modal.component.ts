import { Component, OnInit } from '@angular/core';
import { User } from '@core/models/account/user.model';
import { BookingCreate } from '@core/models/booking/booking-create.model';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { Door } from '@core/models/master/door.model';
import { SupplierGroup } from '@core/models/master/supplier.model';
import { WarehouseCapacityDto } from '@core/models/master/warehouse-capacity.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { DoorService } from '@core/services/master/door.service';
import { SupplierService } from '@core/services/master/supplier.service';
import { WarehouseCapacityService } from '@core/services/master/warehouse-capacity.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { AuthService } from 'auth/auth.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Observable, combineLatest, concatMap, map } from 'rxjs';
import { OperationTime } from '@core/models/master/operation-time.model';
import { OperationService } from '@core/services/master/operation.service';
import { Operation } from '@core/models/master/operation.model';
import { OperationFixSlot } from '@core/models/master/operation-fix-slot.model';
import { SlotCapacityService } from '@core/services/booking/slot-capacity.service';

@UntilDestroy()
@Component({
  selector: 'app-slot-time-modal',
  templateUrl: 'slot-time-modal.component.html',
  styleUrls: ['slot-time-modal.component.scss'],
})
export class SlotTimeModalComponent implements OnInit {

  hours = [];

  doors: Door[] = [];
  bookingHeaders: BookingHeader[] = [];
  warehouse$: Observable<Warehouse[]>;
  warehouseSelect: Warehouse;
  warehouses: Warehouse[] = [];

  warehouseCode: string;
  supplierCode: string;
  supplierName: string;
  bookingDate: Date;
  internalSupGroupId: number;
  supplierGroup: SupplierGroup;
  totalWeight:number;

  durationTime: number;  
  warehouseCaps: WarehouseCapacityDto[] = [];

  startTime: Date;
  endTime: Date;

  bookingHdrKeyId: number;
  bookingId: string;
  bookingSlot: BookingCreate;
  allowToCreate: boolean = true;
  minDate: Date = new Date();
  maxDate: Date = new Date();
  minDateTime: Date = new Date();
  maxDateTime: Date = new Date();
  user : User;

  bookingMinDate: Date = new Date();
  isOverCap: boolean;
  operationTimes: OperationTime[];
  isOverCutoff: boolean;

  operationFixSlotByDate: OperationFixSlot[];
  operationFixSlotBySub: OperationFixSlot[];
  operationType: Operation;
  merchType: string;

  slotData: any;
  slotCapacity: any;

  minDateStart: Date = new Date();

  nextShift: Date = new Date();

  isAllOverCap: boolean = false;

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private authService: AuthService,
    private supplierService: SupplierService,
    private doorService: DoorService,
    private bookingHeaderService: BookingHeaderService,
    private warehouseService: WarehouseService,
    private warehouseCapService: WarehouseCapacityService,
    private operationService: OperationService,
    private slotCapacityService: SlotCapacityService
  ) {
    this.warehouseCode = config.data.warehouseCode;
    this.bookingDate = config.data.bookingDate;
    this.supplierCode = config.data.supplierCode;
    this.durationTime = config.data.totalEstTime;
    this.bookingSlot = config.data.bookingSlot;
    this.supplierName = config.data.supplierName;
    this.internalSupGroupId = config.data.internalSupGroupId;

    this.bookingHdrKeyId = config.data.bookingHdrKeyId;
    this.bookingId = config.data.bookingId;

    this.totalWeight = config.data.totalWeight;

    this.bookingMinDate = this.bookingSlot.minBookingDateTime;

    this.nextShift = config.data.bookingSlot.nextShift;

    this.user = this.authService.getUser();

    if(this.totalWeight == undefined){
      this.totalWeight = 0;
    }
    
  }

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(){    
    this.hours = [];

    let totalQty = 0;
    let totalTruck = 0;
    if(this.bookingSlot.bookingDetail !== null && this.bookingSlot.bookingDetail !== undefined){
      totalQty = this.bookingSlot.bookingDetail.reduce((accumulator, item) => accumulator + +item.totalQty, 0);
      totalTruck = this.bookingSlot.bookingTruck.reduce((accumulator, item) => accumulator + +item.totalTruck, 0);
    }

    combineLatest([
      this.slotCapacityService.getSlotCapacity(this.bookingDate,this.warehouseCode,this.bookingSlot.merchType),
      this.warehouseService.getById(this.warehouseCode),
      this.operationService.getOperationTimeByWhseAndOperation(this.warehouseCode,this.bookingSlot.merchType),
      this.operationService.getOperationFixSlotBySupplierGroup(this.bookingSlot.merchType,this.warehouseCode,this.internalSupGroupId),
      this.operationService.getOperationFixSlot(this.bookingSlot.merchType,this.warehouseCode),
      this.supplierService.getSupGroup(this.internalSupGroupId),
      this.operationService.getByOperationNameAndWhse(this.bookingSlot.merchType,this.warehouseCode)
    ])
    .pipe(untilDestroyed(this))
    .subscribe(([slotData,whseData,optData,optFixSup,optFixDate,supData,optType]) => {
      this.warehouseSelect = whseData;
      this.slotCapacity = slotData;
      this.operationTimes = optData;
      this.operationFixSlotBySub = optFixSup;
      this.operationFixSlotByDate = optFixDate;
      this.supplierGroup = supData;
      this.operationType = optType;
      this.minDate = new Date();
      this.minDateStart = new Date(this.bookingDate);
      let supCutoff = this.supplierGroup.warehouseCutoff.split('|').includes(this.warehouseSelect.warehouseCode) ? true : false;

      let supVip = false;
      if(this.supplierGroup.isVip == "Y" && this.supplierGroup.warehouses.split('|').includes(this.warehouseSelect.warehouseCode)){
        supVip = true;
      }

      this.minDateStart = this.nextShift;
      this.minDate = this.nextShift;

      // Convert date strings to Date objects for both FixSlotByDate and FixSlotBySub
      this.operationFixSlotByDate.forEach(a => a.startTime = new Date(a.startTime));
      this.operationFixSlotBySub.forEach(a => a.startTime = new Date(a.startTime));

      this.hours = this.slotCapacity.map(slot => {

        slot.timeSlot = new Date(slot.timeSlot);

        this.minDate.setSeconds(0,0);
        slot.timeSlot.setSeconds(0,0);

        // console.log('min: ' + this.minDate);
        // console.log('time slot: ' + slot.timeSlot);
        // console.log('slot cap: ' + slot.cap);
        // console.log('slot cap truck: ' + slot.capTruck);

        const capPercent = slot.cap === 0 ? 0 : (slot.totalQty * 100 / slot.cap);
        const capTruckPercent = slot.capTruck === 0 ? 0 : (slot.totalTruck * 100 / slot.capTruck);
        // let isDisable = slot.cap === 0 || capPercent >= 100 || capTruckPercent >= 100 || slot.timeSlot < this.minDate
        // || +slot.totalQty + totalQty > slot.cap || +slot.totalTruck + totalTruck > slot.capTruck;

        let isOverCap = capPercent >= 100 || capTruckPercent >= 100 || +slot.totalQty + +totalQty > +slot.cap 
        || +slot.totalTruck + +totalTruck > slot.capTruck;
        let isNoCap = slot.cap === 0;
        let isLateTime = slot.timeSlot < this.minDate;
        //let isLateTime = this.bookingMinDate > this.minDate;

        let isDisable = isNoCap || isOverCap || isLateTime;

        let isCutoff = false;

        console.log('slot time' + slot.timeSlot);
        console.log('total qty: ' + totalQty);
        console.log('slot qty: ' + slot.totalQty);

        // console.log('modal slot');
        // console.log(slot.timeSlot);
        // console.log(this.minDate);

        // console.log('min date:' + this.minDate);
        // console.log('slot time:', slot.timeSlot);
        //console.log('booking min date:' + this.bookingMinDate);
        // console.log('slot time:' + slot.startTime);
        // console.log('late time:' + isLateTime);

        let dayNum = this.bookingDate.getDay();
        if(dayNum == 7){
          dayNum = 1;
        }else{
          dayNum = dayNum + 1;
        }

        let checkFixDate = this.operationFixSlotByDate.filter(x=>x.supGroupId == this.internalSupGroupId && x.daysOfWeek == dayNum.toString());
        
        if(checkFixDate.length > 0)
        {          
          let isFix = checkFixDate.find(x=>x.startTime.getHours().toString().padStart(2,'0') == 
          (slot.operationTime.split(':')[0]).toString().padStart(2,'0')
          && x.startTime.getMinutes().toString().padStart(2,'0') == 
          (slot.operationTime.split(':')[1]).toString().padStart(2,'0')
          );

          // VIP supplier logic
          //if(this.supplierGroup.isVip == "Y"){
          if(supVip){  
            isDisable = isFix !== undefined ? false : true;
            isNoCap = isFix !== undefined ? false : true;
            isOverCap = isFix !== undefined ? false : true;
          }
          else{ // fix slot but not VIP
            isDisable = isFix !== undefined && slot.cap > 0 ? false : true;
          }
        }          

        if(this.user.userType !== "SUP" && this.user.userType !== "SUPTRAN" && this.user.userType !== "BH" && this.user.userType !== 'DEVELOP'){
          isOverCap = false;
          isDisable = false;
        }
        
        let curDate = new Date();
        curDate.setHours(0,0,0,0);

        if((this.user.userType == "CONTROL" || this.user.userType == "ADMIN" ) && curDate <= this.bookingDate ){
          isLateTime = false;
        }
        
        if((this.operationType.overcutoff == 1 && slot.timeSlot > new Date() && isLateTime && supCutoff)){
          isLateTime = false;
          isDisable = false;
          isCutoff = true;
          isOverCap = false;
        }
        else{
          isCutoff = false;
        }

      
        // console.log('over cutoff:' + isCutoff);
        // console.log('slot time' + slot.timeSlot);
        // console.log('qty: ' + slot.cap);
        // console.log('truck:' + slot.capTruck);
        // console.log('is disable: ' + isDisable);
        // console.log('is no capacity setup: ' + isNoCap);
        // console.log('is Over: ' + isOverCap);
        // console.log('is late:',isLateTime);
        // console.log('cap: ' + capPercent);
        // console.log('captruck: ' + capTruckPercent);
        // console.log('total Qty: ' + (slot.totalQty + totalQty));
        // console.log('total Truck: ' + (slot.totalTruck));
        // console.log('total Truck: ' + (+totalTruck));
               
        console.log('-----------------------------');
        
        return {
          label: slot.operationTime,
          capValue: slot.cap,
          capTruckValue: slot.capTruck,
          capUseValue: slot.totalQty,
          capTruckUseValue: slot.totalTruck,
          capPercent: capPercent.toFixed(0),
          capTruckPercent: capTruckPercent.toFixed(0),
          disabled: isDisable,
          isOverCap: isOverCap,
          isNoCap: isNoCap,
          isLate: isLateTime,
          isOverCutoff: isCutoff
        };
      });

      // check all time slot is over cap
      if(this.hours.filter(x=>x.isOverCap == false && x.isNoCap == false).length > 0){
        this.isAllOverCap = false;
      }
      else{
        this.isAllOverCap = this.operationType.overcap == 1 ? true : false;      
      }

    });

  }

  onTimeStartChange(){
    if(this.durationTime == 0){
      this.durationTime = 0; 
    }

    let hour = this.durationTime/60;
    let min = this.durationTime%60;

    this.endTime = new Date(this.startTime);
    this.endTime.setHours(this.endTime.getHours() + hour);
    this.endTime.setMinutes(this.endTime.getMinutes() + min);
  }

  onTimeClick(time:string,isOver: boolean,isCutoff: boolean){
    let hour = +time.split(':')[0];
    let min = +time.split(':')[1];
    
    this.isOverCap = isOver;
    this.startTime = new Date(this.bookingDate);
    this.isOverCutoff = isCutoff;

    if (this.warehouseSelect.firstTimeOfDay > this.warehouseSelect.endTimeOfDay){
      if(hour >= this.warehouseSelect.firstTimeOfDay){
        this.startTime.setDate(this.startTime.getDate()-1);
      }
    }
    this.startTime.setHours(hour);
    this.startTime.setMinutes(min);
    this.startTime.setSeconds(0);

    this.onTimeStartChange();
  }

  onClose() {
    this.ref.close();
  }

  confirm(){
    this.slotData = { StartTime: this.startTime, EndTime: this.endTime, BookingId: this.bookingId, BookingHdrKeyId: this.bookingHdrKeyId, OverCap: this.isOverCap,OverCutoff: this.isOverCutoff };
    this.ref.close(this.slotData);
  }

  canConfirm():boolean{
    if (this.startTime.toISOString() === this.endTime.toISOString()){
      return true;
    }
    else{
      return false;
    }
  }

}
