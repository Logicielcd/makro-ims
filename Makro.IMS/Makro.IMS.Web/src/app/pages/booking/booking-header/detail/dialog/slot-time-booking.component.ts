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
import { UntilDestroy } from '@ngneat/until-destroy';
import { AuthService } from 'auth/auth.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Observable, concatMap, map } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'app-slot-time-booking-modal',
  templateUrl: 'slot-time-booking.component.html',
})
export class SlotTimeBookingModalComponent implements OnInit {
  hours = [
    { label: '01:00', value: Math.floor(Math.random() * 100) },
    { label: '02:00', value: Math.floor(Math.random() * 100) },
    { label: '03:00', value: Math.floor(Math.random() * 100) },
    { label: '04:00', value: Math.floor(Math.random() * 100) },
    { label: '05:00', value: Math.floor(Math.random() * 100) },
    { label: '06:00', value: Math.floor(Math.random() * 100) },
    { label: '07:00', value: Math.floor(Math.random() * 100) },
    { label: '08:00', value: Math.floor(Math.random() * 100) },
    { label: '09:00', value: Math.floor(Math.random() * 100) },
    { label: '10:00', value: Math.floor(Math.random() * 100) },
    { label: '11:00', value: Math.floor(Math.random() * 100) },
    { label: '12:00', value: Math.floor(Math.random() * 100) },
    { label: '13:00', value: Math.floor(Math.random() * 100) },
    { label: '14:00', value: Math.floor(Math.random() * 100) },
    { label: '15:00', value: Math.floor(Math.random() * 100) },
    { label: '16:00', value: Math.floor(Math.random() * 100) },
    { label: '17:00', value: Math.floor(Math.random() * 100) },
    { label: '18:00', value: Math.floor(Math.random() * 100) },
    { label: '19:00', value: Math.floor(Math.random() * 100) },
    { label: '20:00', value: Math.floor(Math.random() * 100) },
    { label: '21:00', value: Math.floor(Math.random() * 100) },
    { label: '22:00', value: Math.floor(Math.random() * 100) },
    { label: '23:00', value: Math.floor(Math.random() * 100) },
    { label: '24:00', value: Math.floor(Math.random() * 100) },
  ];

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

  slotData: any;

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private authService: AuthService,
    private supplierService: SupplierService,
    private doorService: DoorService,
    private bookingHeaderService: BookingHeaderService,
    private warehouseService: WarehouseService,
    private warehouseCapService: WarehouseCapacityService
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

    this.minDate = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()-1);
    this.maxDate = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate());

    this.minDateTime = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()-1,22,0,0);
    this.maxDateTime = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate(),22,0,0);

    this.user = this.authService.getUser();

  }

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(){
    this.warehouseService.getAll().pipe(
      concatMap(whse => this.doorService.getByWarehouseAndSupCode(this.warehouseCode,this.supplierCode).pipe(
        map(door => {
          this.warehouses = whse;
          this.warehouseSelect = whse.find(x=>x.warehouseCode == this.warehouseCode);
          this.doors = door;
          ({whse,door})
        })
      )),
      concatMap(whse_door => this.bookingHeaderService.getDashboard(this.warehouseCode).pipe(
        map(dh => (this.bookingHeaders = dh))
      )),
      concatMap(dash => this.warehouseCapService.getBookingCapacity(this.warehouseCode,this.bookingDate).pipe(
        map(whseCap => (this.warehouseCaps = whseCap))
      )),
      concatMap(cap_dash => this.supplierService.getSupGroup(this.internalSupGroupId).pipe(
        map(supGroup => (this.supplierGroup = supGroup))
      ))
    ).subscribe(ret=>{
      // this.createBooking();
      // this.createDoors();
      this.warehouseCaps.forEach(data => {
        data.bookingDateTime = new Date(data.bookingDateTime);
      });

      this.hours = [];
      
      this.startTime = new Date(this.bookingDate);
      this.endTime = new Date(this.bookingDate);
      
      this.startTime.setHours(0,0,0,0);
      this.endTime.setHours(0,0,0,0);

      this.warehouseCaps = this.warehouseCaps.sort((a,b) => 
      {
        if (a.bookingDateTime > b.bookingDateTime)
          return 1;
        if (a.bookingDateTime<b.bookingDateTime)
          return -1;
        return 0;
      });

      this.warehouseCaps.forEach(cap => {
        let capPercent = (cap.cubE_CON / cap.maX_CUBE_CON) * 100;
        let capTime = cap.bookingDateTime.getHours().toString().padStart(2,"0") + ":00";
        this.hours.push({label: capTime, value: capPercent});
      });

    });
  }

  onTimeStartChange(){

    if(this.durationTime == 0){
      this.durationTime = 30; 
    }

    let hour = this.durationTime/60;
    let min = this.durationTime%60;

    this.endTime = new Date(this.startTime);

    this.endTime.setDate(this.startTime.getDate());
    this.endTime.setHours(this.endTime.getHours() + hour);
    this.endTime.setMinutes(this.endTime.getMinutes() + min);

  }

  onClose() {
    this.ref.close();
  }

  confirm(){
    this.slotData = { StartTime: this.startTime, EndTime: this.endTime }
    this.ref.close(this.slotData);
  }

}
