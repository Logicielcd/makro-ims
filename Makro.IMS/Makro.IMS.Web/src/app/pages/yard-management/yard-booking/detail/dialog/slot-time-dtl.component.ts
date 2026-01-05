import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { extend, isNullOrUndefined } from '@syncfusion/ej2-base';

import { DateTimePicker } from '@syncfusion/ej2-calendars';
import { MultiSelect } from '@syncfusion/ej2-dropdowns';

import { ActionEventArgs, DragAndDropService, EventRenderedArgs, EventSettingsModel, GroupModel, PopupCloseEventArgs, PopupOpenEventArgs, RenderCellEventArgs, ResizeService, ScheduleComponent, TimelineViewsService, TimeScaleModel, timezoneData, View, WorkHoursModel } from '@syncfusion/ej2-angular-schedule';

import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';

import { Door } from '@core/models/master/door.model';
import { DoorService } from '@core/services/master/door.service';

import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseCapacityService } from '@core/services/master/warehouse-capacity.service';
import { WarehouseService } from '@core/services/master/warehouse.service';

import { BookingHeader } from '@core/models/booking/booking-header.model';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';

import { SupplierGroup } from '@core/models/master/supplier.model';
import { SupplierService } from '@core/services/master/supplier.service';

import { BookingCreate } from '@core/models/booking/booking-create.model';
import { WarehouseCapacityDto } from '@core/models/master/warehouse-capacity.model';
import { UntilDestroy } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { concatMap, map, Observable, switchMap } from 'rxjs';

@UntilDestroy()

@Component({
  //selector: 'control-content',
  templateUrl: 'slot-time-dtl.component.html',
  styleUrls: ['slot-time-dtl.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TimelineViewsService, ResizeService, DragAndDropService]
})

export class SlotTimeDtlComponent implements OnInit {

  public selectedDate: Date = new Date();
  public timeScale: TimeScaleModel = {enable:true, interval: 60, slotCount: 2 };
  public workHours: WorkHoursModel = { start: '00:00', end: '23:59' };
  public currentView: View = 'TimelineWorkWeek'; //'TimelineDay';
  public workDays: number[] = [4,5];
  public showQuickInfo: Boolean = false;
  public minDate: Date = new Date();
  public maxDate: Date = new Date();
  public minDateTime: Date = new Date();
  public maxDateTime: Date = new Date();
  public group: GroupModel = {
    enableCompactView: true,
    resources: ['Booking']
  };

  public timezone: any = timezoneData.find(x=>x.Value == 'Asia/Bangkok');

  doors: Door[] = [];
  bookingHeaders: BookingHeader[] = [];

  warehouse$: Observable<Warehouse[]>;
  warehouseSelect: Warehouse;
  warehouses: Warehouse[] = [];

  warehouseCode: string;
  supplierCode: string;
  internalSupGroupId: number;
  bookingDate: Date;

  scheduleStartTime: Date;
  scheduleEndTime: Date;

  durationTime: number;
  warehouseCaps: WarehouseCapacityDto[] = [];

  bookingHdrKeyId: number;
  bookingId: string;
  bookingSlot: BookingCreate;

  public resourceDataSource: Record<string,any>[] = [];
  public bookingDataSource: Record<string,any>[] = [];

  public newBookingData: BookingHeader;

  public allowMultiple = false;

  supplierGroup: SupplierGroup;

  alertFixSlot: boolean;

  user : User;

  slotData: any;

  public eventSettings: EventSettingsModel = {
    dataSource: extend([], this.bookingDataSource, null, true) as Record<string, any>[],
    fields: {
      id: 'Id',
      subject: { title: 'Summary', name: 'Subject' },
      location: { title: 'Location', name: 'Location' },
      description: { title: 'Comments', name: 'Description' },
      startTime: { title: 'From', name: 'StartTime', validation: { required: true }  },
      endTime: { title: 'To', name: 'EndTime', validation: { required: true, range: [
        this.customEndFn,
        'The end date less than or equal the start date',
      ], } }
    },
    enableTooltip: true,
  };

  @ViewChild('scheduleObj') public scheduleObj: ScheduleComponent;

  constructor(private doorService: DoorService
    ,private bookingHeaderService: BookingHeaderService
    ,private warehouseService: WarehouseService
    ,private warehouseCapService: WarehouseCapacityService
    ,public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private authService: AuthService
    ,private supplierService: SupplierService
    )
  {

      this.alertFixSlot = false;
      this.warehouseCode = config.data.warehouseCode;
      this.bookingDate = config.data.bookingDate;
      this.supplierCode = config.data.supplierCode;
      this.durationTime = config.data.totalEstTime;
      this.bookingHdrKeyId = config.data.bookingHdrKeyId;
      this.bookingId = config.data.bookingId;
      this.bookingSlot = config.data.bookingSlot;
      this.internalSupGroupId = config.data.internalSupGroupId;

      this.minDate = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()-1);
      this.maxDate = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate());

      this.minDateTime = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()-1,22,0,0);
      this.maxDateTime = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate(),22,0,0);

      this.user = this.authService.getUser();

  }

  ngOnInit()
  {
    this.selectedDate = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate());

    this.workDays = [];
    this.workDays.push(this.bookingDate.getDay()-1);
    this.workDays.push(this.bookingDate.getDay());

    this.fetchData();
  }

  fetchData() : void
  {

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
        map(supGroup => (
          this.supplierGroup = supGroup
        ))
      ))
    ).subscribe(ret=>{
      this.createBooking();
      this.createDoors();
      this.warehouseCaps.forEach(data => {
        data.bookingDateTime = new Date(data.bookingDateTime);
      });
      if(this.supplierGroup.fixTimeStart.length > 0){
        this.alertFixSlot = true;
      }
    });

  }

  getBookingData() : void
  {
    this.bookingHeaderService.getDashboard(this.warehouseSelect.warehouseCode)
    .pipe(
    switchMap(booking => {
      this.bookingHeaders = booking;
      return this.doorService.getByWarehouse(this.warehouseSelect.warehouseCode)
    })
    )
    .subscribe(data=> {
      this.doors = data;
      this.createBooking();
      this.createDoors();
    });
  }

  createBooking(): void
  {
    this.bookingDataSource = [];

    this.bookingHeaders.forEach(booking => {

      let supCode: string = "";
      let isBlock: boolean = false;
      let colorCode: number = -32640;
      let fontColorCode: number = -16777216;
      let isReadOnly: boolean = false;
      if(this.user.userType != "SUP" || this.supplierCode == booking.supCode){
        supCode = booking.supName;
        colorCode = -5932640;
        fontColorCode = -16777216;
        isReadOnly = true;
      }
      else if (booking.supCode != this.supplierCode){
        supCode = "UnAvailable time slot";
        isBlock = true;
      }

      if(booking.bookingId == this.bookingId){
        isReadOnly = false;
      }

      if(booking.bookingId == this.bookingSlot.bookingId){
        colorCode = -300100099;
        this.bookingDataSource.push({Id: this.bookingSlot.internalHeaderKey, DoorId: this.bookingSlot.internalDoorId, Subject: supCode
          , Location: supCode,Description: this.bookingSlot.bookingId,StartTime: this.bookingSlot.startTime,EndTime: this.bookingSlot.endTime
          , CategoryColor: this.toColor(colorCode), FontColor: this.toColor(fontColorCode),IsBlock: isBlock,IsReadonly: isReadOnly
        })
      }
      else{
        this.bookingDataSource.push({Id: booking.internalHeaderKey, DoorId: booking.internalDoorId, Subject: supCode
          , Location: supCode,Description: booking.bookingId,StartTime: booking.bookingStart,EndTime: booking.bookingEnd
          , CategoryColor: this.toColor(colorCode), FontColor: this.toColor(fontColorCode),IsBlock: isBlock,IsReadonly: isReadOnly
        })
      }

    });

    let blockStart: Date = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()-1,0,0);
    let blockEnd:Date = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate()-1,21,59);

    let blockStart2: Date = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate(),22,0);
    let blockEnd2:Date = new Date(this.bookingDate.getFullYear(),this.bookingDate.getMonth(),this.bookingDate.getDate(),24,0);


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

  createDoors(): void
  {
    this.resourceDataSource = [];
    this.doors.forEach(door => {
        this.resourceDataSource.push({text: door.doorName,id: door.internalDoorId, color: '#865fcf', capacity: 20, type: door.warehouseCode })
    });
    this.scheduleObj.refresh();
  }

  public isReadOnly(endDate: Date): boolean
  {
    return (endDate < new Date(2022, 8, 1, 0, 0));
  }

  public onPopupOpen(args: PopupOpenEventArgs): void
  {    

    if (args.type === 'Editor') {

      if(this.bookingHdrKeyId > 0 && args.data.Id === undefined){
        args.cancel = true;
      }

      // check available slot
      if (args.target.classList.contains('e-read-only-cells') ||
        !this.scheduleObj.isSlotAvailable(args.data))
      {
        args.cancel = true;
        alert('This slot is already booked');
      }

      args.duration = this.durationTime;

      let startElement: HTMLInputElement = args.element.querySelector('#StartTime') as HTMLInputElement;
      let endElement: HTMLInputElement = args.element.querySelector('#EndTime') as HTMLInputElement;

      if (!startElement.classList.contains('e-datetimepicker')) {
          new DateTimePicker({ value: new Date(startElement.value) || new Date() }, startElement);
      }

      let endTime:Date = new Date(startElement.value);
      let hour = this.durationTime/60;
      let min = this.durationTime%60;

      if(args.data.Id === undefined){
        endTime.setHours(endTime.getHours() + hour);
        endTime.setMinutes(endTime.getMinutes() + min);
      }
      else{
        endTime = new Date(endElement.value);
      }

      if (!endElement.classList.contains('e-datetimepicker')) {
          // new DateTimePicker({ value: new Date(endElement.value) || new Date() }, endElement);
          new DateTimePicker({ value: new Date(endTime) || new Date() }, endElement);
      }


      let processElement: HTMLInputElement= args.element.querySelector('#DoorId');
      if (!processElement.classList.contains('e-multiselect')) {
          let multiSelectObject: MultiSelect = new MultiSelect({
              placeholder: 'Choose a Door',
              fields: { text: 'text', value: 'id'},
              dataSource: <any>this.resourceDataSource,
              value: <string[]>((args.data.DoorId instanceof Array) ? args.data.DoorId : [args.data.DoorId]),
              readonly: true
          });
          multiSelectObject.appendTo(processElement);
      }

    }
  }

  onPopupClose(args: PopupCloseEventArgs) : void {
    if (args.type === 'Editor' && !isNullOrUndefined(args.data)) {
        let subjectElement: HTMLInputElement = args.element.querySelector('#Subject') as HTMLInputElement;
        if (subjectElement ) {
            (<{ [key: string]: Object }>(args.data)).Subject = subjectElement.value;
        }
        let statusElement: HTMLInputElement = args.element.querySelector('#DoorId') as HTMLInputElement;
        if (statusElement) {
            ((<{ [key: string]: Object }>(args.data)).EventType as string) = statusElement.value;
        }
        let startElement: HTMLInputElement = args.element.querySelector('#StartTime') as HTMLInputElement;
        if (startElement) {
            (<{ [key: string]: Object }>(args.data)).StartTime = new Date(startElement.value);
        }
        let endElement: HTMLInputElement = args.element.querySelector('#EndTime') as HTMLInputElement;
        if (endElement) {
            (<{ [key: string]: Object }>(args.data)).EndTime = new Date(endElement.value);
        }
        let descriptionElement: HTMLInputElement = args.element.querySelector('#Description') as HTMLInputElement;
        if (descriptionElement) {
            ((<{ [key: string]: Object }>(args.data)).Description as string) = descriptionElement.value;
        }

        this.slotData = args.data;

        // if(this.alertFixSlot){
        //   this.validateFixtimeSlot(args);
        // }

        //this.ref.close(args.data);
    }
  }

  public onActionBegin(args: ActionEventArgs): void
  {
    if (args.requestType === 'eventCreate' && (this.bookingHdrKeyId === undefined || this.bookingHdrKeyId === null)){
      let data: Record<string, any>;
      data = (args.data[0] as Record<string, any>);
      if (!this.scheduleObj.isSlotAvailable(data)) {
        args.cancel = true;
      }
    }

    if (args.requestType === 'eventChange') {
      let data: Record<string, any>;
      data = (args.data as Record<string, any>);
      if (!this.scheduleObj.isSlotAvailable(data)) {
        args.cancel = true;
      }
      else{
        ((<{ [key: string]: Object }>(args.data)).EventType as string) = data.DoorId;

        this.slotData = args.data;

        if(this.alertFixSlot){
          this.validateFixtimeSlot(args);
        }

        // this.ref.close(args.data);
      }
    }
  }

  validateFixtimeSlot(args: any){
         // validate booking time and fix slot
         let startFixtime = new Date(this.bookingDate);
         let endFixtime = new Date(this.bookingDate);

         if(+this.supplierGroup.fixTimeStart.split(':')[0] > 21){
           startFixtime.setDate(this.bookingDate.getDate() - 1);
           startFixtime = new Date(startFixtime.getFullYear(), startFixtime.getMonth(),startFixtime.getDate()
           , +this.supplierGroup.fixTimeStart.split(':')[0], +this.supplierGroup.fixTimeStart.split(':')[1])
         }
         else{
           startFixtime = new Date(this.bookingDate.getFullYear(), this.bookingDate.getMonth(),this.bookingDate.getDate()
           , +this.supplierGroup.fixTimeStart.split(':')[0], +this.supplierGroup.fixTimeStart.split(':')[1])
         }

         endFixtime = new Date(this.bookingDate.getFullYear(), this.bookingDate.getMonth(),this.bookingDate.getDate()
         , +this.supplierGroup.fixTimeEnd.split(':')[0], +this.supplierGroup.fixTimeEnd.split(':')[1])

         let startTime = new Date();
         let endTime = new Date();

         startTime = ((<{ [key: string]: Object }>(args.data)).StartTime as Date)
         endTime = ((<{ [key: string]: Object }>(args.data)).EndTime as Date)

         if((startTime < startFixtime || startTime > endFixtime) || (endTime < startFixtime || endTime > endFixtime))
         {
           alert('You are booking not match with you fix slot time');
         }

  }

  public onRenderCell(args: RenderCellEventArgs): void
  {
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
    }

    if (args.element.classList.contains('e-work-cells')) {
      let startTime: Date;
      let endTime: Date;

      if(this.supplierGroup !== null && this.supplierGroup.fixTimeStart !== undefined && this.supplierGroup.fixTimeStart !== null){
        if(+this.supplierGroup.fixTimeStart.split(':')[0] > 21){
          startTime = new Date(this.minDate.getFullYear(),this.minDate.getMonth(), this.minDate.getDate()
                      , +this.supplierGroup.fixTimeStart.split(':')[0], +this.supplierGroup.fixTimeStart.split(':')[1]);
        }
        else{
          startTime = new Date(this.maxDate.getFullYear(),this.maxDate.getMonth(), this.maxDate.getDate()
                      , +this.supplierGroup.fixTimeStart.split(':')[0], +this.supplierGroup.fixTimeStart.split(':')[1]);
        }

        if(+this.supplierGroup.fixTimeEnd.split(':')[0] > 21){
          endTime = new Date(this.minDate.getFullYear(),this.minDate.getMonth(), this.minDate.getDate()
                      , +this.supplierGroup.fixTimeEnd.split(':')[0], +this.supplierGroup.fixTimeEnd.split(':')[1]);
        }
        else{
          endTime = new Date(this.maxDate.getFullYear(),this.maxDate.getMonth(), this.maxDate.getDate()
                      , +this.supplierGroup.fixTimeEnd.split(':')[0], +this.supplierGroup.fixTimeEnd.split(':')[1]);
        }

      }

      if (args.date >= startTime && args.date < endTime){
        (args.element as HTMLElement).style.background = "#A5E6BA";
      }

    }

    if (args.elementType === 'emptyCells' && args.element.classList.contains('e-resource-left-td')) {
      const target: HTMLElement = args.element.querySelector('.e-resource-text') as HTMLElement;
      target.innerHTML = '<div class="name">Dock Door</div><div class="type">Type</div><div class="capacity">Capacity</div>';
      target.innerHTML = '<div class="dockdoor">Dock Door</div>';
    }
  }

  public onEventRendered(args: EventRenderedArgs): void
  {
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

  public onNavigating(args: any): void
  {
    this.selectedDate = args.currentDate;
  }

  customEndFn(args) {

    let startElement: HTMLInputElement = document.querySelector('#StartTime') as HTMLInputElement;
    return startElement.value < args.value;
  }

  public toColor(num)
  {
    num >>>= 0;
    var b = num & 0xFF,
        g = (num & 0xFF00) >>> 8,
        r = (num & 0xFF0000) >>> 16,
        a = ( (num & 0xFF000000) >>> 24 ) / 255 ;
    return "rgba(" + [r, g, b, a].join(",") + ")";
  }

  confirm(){
    this.ref.close(this.slotData);
  }
}
