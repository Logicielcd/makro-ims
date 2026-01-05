import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';

import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { BookingHeaderDto, BookingTruckCheckIn } from '@core/models/booking/booking-header.model';
import { DatePipe,Location } from '@angular/common';
import { GuardCheckInOutDto } from '@core/models/booking/check-in-out.model';
import { GuardCheckInOutService } from '@core/services/booking/check-in.service';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { ConfirmationService, MessageService } from 'primeng/api';

@UntilDestroy()
@Component({  
  templateUrl: 'dialog.component.html',
  styleUrls: ['dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class DialogComponent implements OnInit {

  cols: any[];
  bookingHeader: BookingHeaderDto;
  bookingTrucks: BookingTruckCheckIn[] = [];
  isCheckIn: boolean;
  bookingStart: Date;
  bookingEnd: Date;
  bookingDate: string;
  formattedDate: string;
  bookingNoti: string = '';
  msgs: any[];
  user: User;
  isEarly: boolean;
  statuses: string[];
  
  constructor(
    public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private authService: AuthService
    ,private messageService: MessageService
    ,private bookingHeaderService: BookingHeaderService
    ,private guardCheckInOutService: GuardCheckInOutService
    ,private confirmationService: ConfirmationService    
    ,private _location: Location
    )
  {    
    this.user = this.authService.getUser();
    this.isCheckIn = false;
    this.isEarly = false;
    this.bookingHeader = config.data.bookingHeader;    
    this.bookingTrucks = this.bookingHeader.bookingCheckIns.slice(0);

    this.bookingStart = new Date(this.bookingHeader.bookingStart);
    this.bookingEnd = new Date(this.bookingHeader.bookingEnd);

    this.bookingDate = this.bookingEnd.getDate().toString() + "-" + this.bookingEnd.getMonth().toString();

    const datepipe: DatePipe = new DatePipe('en-US');
    this.formattedDate = datepipe.transform(this.bookingEnd, 'dd/MM/yyyy ');    
    this.formattedDate = this.formattedDate + datepipe.transform(this.bookingStart, 'HH:mm-');
    this.formattedDate = this.formattedDate + datepipe.transform(this.bookingEnd, 'HH:mm ');
  }

  ngOnInit()
  {    
    this.cols = [
      { field: 'truckType', header: 'Truck type' },
      { field: 'licensePlate', header: 'License Plate' },     
      { field: 'driverName', header: 'Driver Name' },      
    ];

    this.statuses.push(...["CHECKIN","QUEUE","CALLTRUCK","ONDOCK","UNLOADING","UNLOADED","LEAVEDOOR","SUBMITDOC"])
    
  }

  confirm(internalTruckCheckInId: number){
    let guardCheckInOut: GuardCheckInOutDto = {} as GuardCheckInOutDto;
          
    guardCheckInOut.bookingId = this.bookingHeader.bookingId;
    guardCheckInOut.internalHeaderKey = this.bookingHeader.internalHeaderKey;
    guardCheckInOut.internalTruckCheckInId = internalTruckCheckInId;
    guardCheckInOut.userName = this.user.name;

    this.guardCheckInOutService.guardCheckOutByTruck(guardCheckInOut)
          .pipe(untilDestroyed(this))
          .subscribe({next: (data) => {            
            this.messageService.add({severity:'success', summary: 'Guard check out completed', detail: '',life: 3000});
            
            this.guardCheckInOutService.getGuardCheckOut(this.bookingHeader.bookingId)
            .pipe(untilDestroyed(this))
            .subscribe({next:(data) =>{
              this.bookingHeader = data;
              this.bookingTrucks = data.bookingCheckIns.slice(0);}
            ,error:(error)=>{
              this.ref.close();
            }});

          },
          error:(error) => {
            this.msgs = [];
            error.Messages.forEach((msg: any) => {
              this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
          }});
        
    this.isCheckIn = true;
    
  }

  confirmEarly(internalTruckCheckInId: number){
    this.confirmationService.confirm({
      message: 'รถมาก่อนเวลานัดหมาย ต้องการให้รถเข้าพื้นที่หรือไม่',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.confirm(internalTruckCheckInId);
      }
    });
  }
}
