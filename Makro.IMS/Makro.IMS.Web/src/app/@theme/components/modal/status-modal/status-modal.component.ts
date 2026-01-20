import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { BookingHeaderDto, BookingTruckCheckIn } from '@core/models/booking/booking-header.model';
import { BookingTruckLog } from '@core/models/booking/booking-truck-log.model';
import { QueueManageDto } from '@core/models/queuemanage/manage-queue.model';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { ManageQueueService } from '@core/services/queuemanage/manage-queue.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { MenuItem, MessageService } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@UntilDestroy()
@Component({
  selector: 'app-status-dialog',
  templateUrl: './status-modal.component.html',
})
export class StatusModalComponent implements OnInit {
  
  items: MenuItem[];
  msgs: any[];
  trackings: any[];

  queue: QueueManageDto;
  bookingHeader: BookingHeaderDto;
  bookingTruck: BookingTruckCheckIn;
  activeIndex: number;

  bookingLogs: BookingTruckLog[];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private manageQueueService: ManageQueueService,
    private bookingHeaderService: BookingHeaderService,
    private messageService: MessageService,
  ) {
    this.queue  = this.config.data.queue;
  }

  ngOnInit(): void {    
    this.bookingHeaderService.getBookingByBookingId(this.queue.bookingId)
    .pipe(untilDestroyed(this))
    .subscribe({ next : (data) => {
      this.bookingHeader = data;
      this.bookingHeader.bookingCheckIns.forEach(truck => {
        truck.checkInTime = truck.checkInTime == null || truck.checkInTime == undefined ? null : new Date(truck.checkInTime);
        truck.arrivedTime = truck.arrivedTime == null || truck.arrivedTime == undefined ? null : new Date(truck.arrivedTime);
        truck.assignQueueTime = truck.assignQueueTime == null || truck.assignQueueTime == undefined ? null : new Date(truck.assignQueueTime);
        truck.ondockTime = truck.ondockTime == null || truck.ondockTime == undefined ? null : new Date(truck.ondockTime);
        truck.startUnloadTime = truck.startUnloadTime == null || truck.startUnloadTime == undefined ? null : new Date(truck.startUnloadTime);
        truck.finishUnloadTime = truck.finishUnloadTime == null || truck.finishUnloadTime == undefined ? null : new Date(truck.finishUnloadTime);
        truck.submitdocTime = truck.submitdocTime == null || truck.submitdocTime == undefined ? null : new Date(truck.submitdocTime);
        truck.calltruckTime = truck.calltruckTime == null || truck.calltruckTime == undefined ? null : new Date(truck.calltruckTime);
        truck.checkoutTime = truck.checkoutTime == null || truck.checkoutTime == undefined ? null : new Date(truck.checkoutTime);
        truck.doccheckTime = truck.doccheckTime == null || truck.doccheckTime == undefined ? null : new Date(truck.doccheckTime);
      });

      this.bookingHeaderService.bookingLog(this.queue.internalTruckCheckInId)
      .pipe(untilDestroyed(this))
      .subscribe((dlog) => {
        this.bookingLogs = dlog;        

        this.bookingLogs.forEach(log => {
          if(log.action == "CREATE")
            log.actionDisplay = "Booking";
          if(log.action == "CHECKIN")
            log.actionDisplay = "Guard check in";
          if(log.action == "QUEUE")
            log.actionDisplay = "Assign queue";
          if(log.action == "CALLTRUCK")
            log.actionDisplay = "Assign truck";
          if(log.action == "ONDOCK")
            log.actionDisplay = "On dock";
          if(log.action == "UNLOADING")
            log.actionDisplay = "Start unload";
          if(log.action == "UNLOADED")
            log.actionDisplay = "End unload";
          if(log.action == "LEAVEDOOR")
            log.actionDisplay = "Leave dock";
          if(log.action == "WAITINGDOC")
            log.actionDisplay = "Waiting document";
          if(log.action == "SUBMITDOC")
            log.actionDisplay = "Confirm document";
          if(log.action == "CHECKOUT")
            log.actionDisplay = "Guard check out";
          if(log.action == "CHANGE DOOR")
            log.actionDisplay = "Change door";
          if(log.action == "CANCEL DOOR")
            log.actionDisplay = "Remove door";
          if(log.action == "CHECKIN PO")
            log.actionDisplay = "Document check";
          if(log.action == "CHECKIN PO NOT IN BOOKING")
            log.actionDisplay = "Document check not in booking";
          if(log.action == "SEND SMS FOR RECEIVED DOCUMENT")
            log.actionDisplay = "Send SMS for received document";
          if(log.action == "CHECKOUT PO")
            log.actionDisplay = "Confirm document";
          if(log.action == "CANCEL PO AFTER UNLOAD")
            log.actionDisplay = "Cancel PO after unloaded";

        });

      });

      this.fetchData();
    },
    error : (error) => {
      this.msgs = [];
              error.Messages.forEach((msg: any) => {
                this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
              });    
    }});
  }

  fetchData(){

    let isRegister: string = '-pass';
    let isGuardCheckIn: string = '';
    let isQueueAssign: string = '';
    let isCallTruck: string = '';
    let isOnDock: string = '';
    let isStartUnload: string = '';
    let isFinishUnload: string = '';
    let isDeparture: string = '';
    let isSubmitDoc: string = '';
    let isGuardCheckOut: string = '';

    const datepipe: DatePipe = new DatePipe('en-US');

    let bookingTruck = this.bookingHeader.bookingCheckIns.find(x=>x.internalTruckCheckInId === this.queue.internalTruckCheckInId);

    this.activeIndex = 0;

    if(bookingTruck.arrivedTime !== null && bookingTruck.arrivedTime !== undefined){
      isGuardCheckIn = '-pass';
      this.activeIndex ++;
    }

    if(bookingTruck.assignQueueTime !== null && bookingTruck.assignQueueTime !== undefined){
      isQueueAssign = '-pass';
      this.activeIndex ++;
    }

    if(bookingTruck.calltruckTime !== null && bookingTruck.calltruckTime !== undefined){      
      isCallTruck = '-pass';
      this.activeIndex ++;
    }

    if(bookingTruck.ondockTime !== null && bookingTruck.ondockTime !== undefined){      
      isOnDock = '-pass';
      this.activeIndex ++;
    }

    if(bookingTruck.startUnloadTime !== null && bookingTruck.startUnloadTime !== undefined){
      isStartUnload = '-pass';
      this.activeIndex ++;
    }

    if(bookingTruck.finishUnloadTime !== null && bookingTruck.finishUnloadTime !== undefined){
      isFinishUnload = '-pass';
      this.activeIndex ++;
    }

    if(bookingTruck.departureTime !== null && bookingTruck.departureTime !== undefined){
      isDeparture = '-pass';
      this.activeIndex ++;
    }

    if(bookingTruck.submitdocTime !== null && bookingTruck.submitdocTime !== undefined){
      isSubmitDoc = '-pass';
      this.activeIndex ++;
    }

    if(bookingTruck.checkoutTime !== null && bookingTruck.checkoutTime !== undefined){
      isGuardCheckOut = '-pass';
      // this.activeIndex ++;
      this.activeIndex = 9;
    }

    this.items = [
      {
        label: 'Booking' + '\r\n ' + (bookingTruck.checkInTime === null ? '' : datepipe.transform(bookingTruck.checkInTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon calendarplus' + isRegister,
      },
      {
        label: 'Guard Check in' + '\r\n ' + (bookingTruck.arrivedTime === null ? '' : datepipe.transform(bookingTruck.arrivedTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon truck' + isGuardCheckIn,
      },
      {
        label: 'Assign queue' + '\r\n ' + (bookingTruck.assignQueueTime === null ? '' : datepipe.transform(bookingTruck.assignQueueTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon ticket' + isQueueAssign,
      },
      {
        label: 'Assign truck' + '\r\n ' + (bookingTruck.calltruckTime === null ? '' : datepipe.transform(bookingTruck.calltruckTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon megaphone' + isCallTruck,
      },
      {
        label: 'On dock' + '\r\n ' + (bookingTruck.ondockTime === null ? '' : datepipe.transform(bookingTruck.ondockTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon signin' + isOnDock,
      },
      {
        label: 'Start unload' + '\r\n ' + (bookingTruck.startUnloadTime === null ? '' : datepipe.transform(bookingTruck.startUnloadTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon box' + isStartUnload,
      },
      {
        label: 'End unload' + '\r\n ' + (bookingTruck.finishUnloadTime === null ? '' : datepipe.transform(bookingTruck.finishUnloadTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon thumbsup' + isFinishUnload,
      },
      {
        label: 'Leave dock' + '\r\n ' + (bookingTruck.departureTime === null ? '' : datepipe.transform(bookingTruck.departureTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon signout' + isDeparture,
      },
      {
        label: 'Confirm document' + '\r\n ' + (bookingTruck.submitdocTime === null ? '' : datepipe.transform(bookingTruck.submitdocTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon file' + isSubmitDoc,
      },
      {
        label: 'Guard Check out' + '\r\n ' + (bookingTruck.checkoutTime === null ? '' : datepipe.transform(bookingTruck.checkoutTime, 'dd/MM/yyyy HH:mm')),
        styleClass: 'icon verify' + isGuardCheckOut,
      },
    ];

  }

}
