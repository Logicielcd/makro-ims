import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { extend, isNullOrUndefined } from '@syncfusion/ej2-base';

import { ActionEventArgs, DragAndDropService, EventRenderedArgs, EventSettingsModel, GroupModel, PopupOpenEventArgs, RenderCellEventArgs, ResizeService, ScheduleComponent, TimelineViewsService, TimeScaleModel, View, WorkHoursModel } from '@syncfusion/ej2-angular-schedule';

import { Door } from '@core/models/master/door.model';
import { DoorService } from '@core/services/master/door.service';

import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseService } from '@core/services/master/warehouse.service';

import { BookingHeader } from '@core/models/booking/booking-header.model';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';

import { WarehouseCapacityDto } from '@core/models/master/warehouse-capacity.model';
import { WarehouseCapacityService } from '@core/services/master/warehouse-capacity.service';

import { NgForm } from '@angular/forms';
import { UntilDestroy } from '@ngneat/until-destroy';
import { concatMap, map, switchMap } from 'rxjs';
import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';

@UntilDestroy()

@Component({
  //selector: 'control-content',
  templateUrl: 'schedule-door.component.html',
  styleUrls: ['schedule-door.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TimelineViewsService, ResizeService, DragAndDropService]
})

export class ScheduleDoorComponent implements OnInit {

  public selectedDate: Date = new Date();
  public timeScale: TimeScaleModel = { enable:true, interval: 60, slotCount: 2 };
  public workHours: WorkHoursModel = { start: '00:00', end: '23:59' };
  public currentView: View = 'TimelineWorkWeek'; // 'TimelineDay';
  public workDays: number[] = [4,5];
  public minDate: Date = new Date();
  public maxDate: Date = new Date();
  public minDateTime: Date = new Date();
  public maxDateTime: Date = new Date();
  public weekFirstDay: number = 3;
  public group: GroupModel = {
    enableCompactView: true,
    resources: ['Booking']
  };

  doors: Door[] = [];
  bookingHeaders: BookingHeader[] = [];

  warehouseSelected: string;
  warehouses: Warehouse[] = [];
  warehouseCaps: WarehouseCapacityDto [] = [];

  user : User;

  public resourceDataSource: Record<string,any>[] = [];
  public bookingDataSource: Record<string,any>[] = [];

  public newBookingData: BookingHeader;

  public allowMultiple = true;

  public eventSettings: EventSettingsModel = {
    dataSource: extend([], this.bookingDataSource, null, true) as Record<string, any>[],
    fields: {
      id: 'Id',
      subject: { title: 'Summary', name: 'Subject' },
      location: { title: 'Location', name: 'Location' },
      description: { title: 'Comments', name: 'Description' },
      startTime: { title: 'From', name: 'StartTime' },
      endTime: { title: 'To', name: 'EndTime' }
    },
    enableTooltip: true,
  };

  @ViewChild('f') f: NgForm;

  @ViewChild('scheduleObj') public scheduleObj: ScheduleComponent;

  constructor(private doorService: DoorService
    ,private bookingHeaderService: BookingHeaderService
    ,private warehouseService: WarehouseService
    ,private warehouseCapacityService: WarehouseCapacityService
    ,private authService: AuthService
    ) {
      
      this.minDate = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth(),this.selectedDate.getDate()-7);
      this.maxDate = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth(),this.selectedDate.getDate()+14);
  
      this.minDateTime = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth(),this.selectedDate.getDate()-1,22,0,0);
      this.maxDateTime = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth(),this.selectedDate.getDate(),22,0,0);
  
      this.selectedDate = new Date();
      this.selectedDate.setDate(this.selectedDate.getDate()+1);
  
      this.user = this.authService.getUser();
  }

  ngOnInit() {
    
    this.workDays = [];

    if(this.selectedDate.getDay() === 0){
      this.weekFirstDay = 6;
      this.workDays.push(6);
      this.workDays.push(this.selectedDate.getDay());
    }
    else{
      this.weekFirstDay = this.selectedDate.getDay() - 1;
      this.workDays.push(this.selectedDate.getDay()-1);
      this.workDays.push(this.selectedDate.getDay());
    }

    this.fetchData();
  }

  fetchData() : void{

    this.warehouseService.getAll().pipe(
      concatMap(whse => this.doorService.getByWarehouse(whse[0].warehouseCode).pipe(
        map(door => {
          this.warehouses = whse;
          this.warehouseSelected = whse[0].warehouseCode;
          this.doors = door;
          ({whse,door})
        })
      )),
      concatMap(whse_door => this.bookingHeaderService.getDashboard(this.warehouseSelected).pipe(
        map(dh => (this.bookingHeaders = dh))
      )),
      concatMap(dash => this.warehouseCapacityService.getBookingCapacity(this.warehouseSelected,this.selectedDate).pipe(
        map(whseCap => (this.warehouseCaps = whseCap))
      ))
    ).subscribe(ret=>{
      this.createBooking();
      this.createDoors();
      this.warehouseCaps.forEach(data => {
        data.bookingDateTime = new Date(data.bookingDateTime);
      });
    });

  }

  getBookingData() : void{
       this.bookingHeaderService.getDashboard(this.warehouseSelected)
       .pipe(
        switchMap(booking => {
          this.bookingHeaders = booking;
          return this.doorService.getByWarehouse(this.warehouseSelected)
        })
       )
       .subscribe(data=> {
        this.doors = data;
        this.createBooking();
        this.createDoors();
       });
  }

  createBooking(): void{
    
    this.bookingDataSource = [];
    this.bookingHeaders.forEach(booking => {      

      let supCode: string = "";
      let isBlock: boolean = false;
      let colorCode: number = -32640;
      let fontColorCode: number = -16777216;
      let isReadOnly: boolean = false;
      let location: string = "";

      if(this.user.userType != "SUP" || this.user.supCode == booking.supCode){
        supCode = booking.supName;
        location = "";
        colorCode = -5932640;
        fontColorCode = -16777216;
        isReadOnly = true;
      }
      else if (booking.supCode != this.user.supCode){
        supCode = "UnAvailable time slot";
        location = "";
        //colorCode= -5932640;
        isBlock = false;
        isReadOnly = true;
      }

      this.bookingDataSource.push({Id: booking.internalHeaderKey, DoorId: booking.internalDoorId, Subject: supCode
        , Location: location,Description: booking.bookingId,StartTime: booking.bookingStart,EndTime: booking.bookingEnd
        , CategoryColor: this.toColor(colorCode), FontColor: this.toColor(fontColorCode), IsReadonly:isReadOnly,IsBlock:isBlock
      })
    });


    let blockStart: Date = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth(),this.selectedDate.getDate()-1,0,0);
    let blockEnd:Date = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth(),this.selectedDate.getDate()-1,21,59);

    let blockStart2: Date = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth(),this.selectedDate.getDate(),22,0);
    let blockEnd2:Date = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth(),this.selectedDate.getDate(),24,0);

    // create block time for booking
    this.doors.forEach((door)=>{
      this.bookingDataSource.push({Id: 0, DoorId: door.internalDoorId, Subject: 'Not available'
        , StartTime: blockStart ,EndTime: blockEnd
        , IsBlock: true
      });
      this.bookingDataSource.push({Id: 0, DoorId: door.internalDoorId, Subject: 'Not available'
        , StartTime: blockStart2 ,EndTime: blockEnd2
        , IsBlock: true
      });
    });


    this.scheduleObj.eventSettings.dataSource = this.bookingDataSource;
  }

  createDoors(): void{
    this.resourceDataSource = [];
    this.doors.forEach(door => {
        this.resourceDataSource.push({text: door.doorName,id: door.internalDoorId, color: '#865fcf', capacity: 20, type: door.warehouseCode })
    });

    this.scheduleObj.refresh();
  }

  public isReadOnly(endDate: Date): boolean {
    return (endDate < new Date(2022, 9, 1, 0, 0));
  }

  public onPopupOpen(args: PopupOpenEventArgs): void {

    if(args.type === 'QuickInfo')
    {
      args.cancel = true;
    }
    else{
      args.cancel = true;
    }

    const data: Record<string, any> = args.data as Record<string, any>;
    if (args.type === 'QuickInfo' || args.type === 'Editor' || args.type === 'RecurrenceAlert' || args.type === 'DeleteAlert') {
      const target: HTMLElement = (args.type === 'RecurrenceAlert' ||
        args.type === 'DeleteAlert') ? args.element[0] : args.target;
      if (!isNullOrUndefined(target) && target.classList.contains('e-work-cells')) {
        if ((target.classList.contains('e-read-only-cells')) ||
          (!this.scheduleObj.isSlotAvailable(data))) {
          args.cancel = true;
        }
      } else if (!isNullOrUndefined(target) && target.classList.contains('e-appointment') &&
        (this.isReadOnly(data.EndTime as Date))) {
        args.cancel = true;
      }
    }
  }

  public onActionBegin(args: ActionEventArgs): void {
    
    if (args.requestType === 'eventCreate' || args.requestType === 'eventChange') {
      let data: Record<string, any>;
      if (args.requestType === 'eventCreate') {
        data = (args.data[0] as Record<string, any>);
      } else if (args.requestType === 'eventChange') {
        data = (args.data as Record<string, any>);
      }
      if (!this.scheduleObj.isSlotAvailable(data)) {
        args.cancel = true;
      }
    }
  }

  public onRenderCell(args: RenderCellEventArgs): void {

    if(args.elementType === 'majorSlot' || args.elementType === 'minorSlot')
    {
      let cubCon = this.warehouseCaps.find(x=>x.bookingDateTime.getDate() === args.date.getDate() && x.bookingDateTime.getHours() === args.date.getHours());

      if(cubCon !== null && cubCon !== undefined){

        let cubPercent = (cubCon.cubE_CON / cubCon.maX_CUBE_CON) * 100;

        if(cubPercent<=85){
          (args.element as HTMLElement).style.background = "white";
        }
        else if(cubPercent > 85 && cubPercent <= 90){
          (args.element as HTMLElement).style.background = "green";
        }
        else if(cubPercent > 90 && cubPercent <= 95){
          (args.element as HTMLElement).style.background = "yellow";
        }
        else if(cubPercent > 95 && cubPercent <= 100){
          (args.element as HTMLElement).style.background = "orange";
        }
        else if(cubPercent > 100){
          (args.element as HTMLElement).style.background = "red";
        }
      }
      else{
        (args.element as HTMLElement).style.background = "white";
      }

      // get capacity

      // console.log(args.elementType);
      //  console.log(args.date);

    }

    if (args.element.classList.contains('e-work-cells')) {
      if (args.date < new Date(2021, 6, 31, 0, 0)) {
        args.element.setAttribute('aria-readonly', 'true');
        args.element.classList.add('e-read-only-cells');
      }
    }
    if (args.elementType === 'emptyCells' && args.element.classList.contains('e-resource-left-td')) {
      const target: HTMLElement = args.element.querySelector('.e-resource-text') as HTMLElement;
      target.innerHTML = '<div class="name">Dock Door</div><div class="type">Type</div><div class="capacity">Capacity</div>';
      target.innerHTML = '<div class="dockdoor">Dock Door</div>';
    }

  }

  public onEventRendered(args: EventRenderedArgs): void {
    const data: Record<string, any> = args.data;
    if (this.isReadOnly(data.EndTime as Date)) {
      args.element.setAttribute('aria-readonly', 'true');
      args.element.classList.add('e-read-only');
    }

    let categoryColor: string = args.data.CategoryColor as string;
    let fontColor: string = args.data.FontColor as string;
    if (!args.element || !categoryColor) {
        return;
    }
    if (this.scheduleObj.currentView === 'Agenda') {
        (args.element.firstChild as HTMLElement).style.borderLeftColor = categoryColor;
    } else {
        args.element.style.backgroundColor = categoryColor;
        args.element.style.color = fontColor;
    }

  }

  public onNavigating(args: any): void {
    // console.log('on navigating');
    // console.log(args.currentDate);
    // console.log(this.warehouseSelected);
    // console.log('Schedule <b>Navigating</b> event is triggered<hr>');

    // this.workDays = [];
    // this.workDays.push(args.currentDate.getDay()-1);
    // this.workDays.push(args.currentDate.getDay());
    this.createBooking();

    this.selectedDate = args.currentDate;

    this.workDays = [];

    if(this.selectedDate.getDay() === 0){
      this.weekFirstDay = 6;
      this.workDays.push(6);
      this.workDays.push(this.selectedDate.getDay());
    }
    else{
      this.weekFirstDay = this.selectedDate.getDay() - 1;
      this.workDays.push(this.selectedDate.getDay()-1);
      this.workDays.push(this.selectedDate.getDay());
    }


  }

  public onWhseChange(event: any):void {
//    console.log(this.warehouseSelected);
    this.getBookingData();
  }

  public toColor(num) {
    num >>>= 0;
    var b = num & 0xFF,
        g = (num & 0xFF00) >>> 8,
        r = (num & 0xFF0000) >>> 16,
        a = ( (num & 0xFF000000) >>> 24 ) / 255 ;
    return "rgba(" + [r, g, b, a].join(",") + ")";
  }

}
