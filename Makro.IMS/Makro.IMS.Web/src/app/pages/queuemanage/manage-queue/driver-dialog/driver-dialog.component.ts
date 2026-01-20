import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { AuthService } from 'auth/auth.service';

import { User } from '@core/models/account/user.model';
import { QueueManageDto, QueueSequence } from '@core/models/queuemanage/manage-queue.model';

import { ConfirmationService, MessageService } from 'primeng/api';
import { ManageQueueService } from '@core/services/queuemanage/manage-queue.service';
import { BookingTruckCheckIn, CheckIn, PoCheckInOut } from '@core/models/booking/booking-header.model';

@UntilDestroy()
@Component({  
  templateUrl: 'driver-dialog.component.html',
  styleUrls: ['driver-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class DriverDialogComponent implements OnInit {

  user: User;
  queue: QueueManageDto;
  data: BookingTruckCheckIn;
  
  isDisable: boolean;

  constructor(
    public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private authService: AuthService
    ,private messageService: MessageService    
    ,private manageQueueService: ManageQueueService
    ,private confirmationService: ConfirmationService
    )
  {      
    this.user = authService.getUser();
    this.queue = config.data.queue;    
  }

  ngOnInit()
  {        
    this.isDisable = true;
    this.manageQueueService.getBookingTruckCheckIn(this.queue.internalTruckCheckInId)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data)=>{
      this.data = data;
    },error: (error)=>{
      error.Messages.forEach((msg: any) => {
        this.messageService.add({key:'dg1', severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }}); 
  }

  onSaveDriverInfo(){

    //validate tel no.
    const phonePattern = /^0\d{9}$/;
    const phoneMatch = this.data.telNo.match(phonePattern);
    if(phoneMatch === null){      
      this.messageService.add({key:'dg1', severity:'error', summary: 'Error', detail: 'Tel No. ผิดรูปแบบ', life: 3000});      
    }
    else{
      this.confirmationService.confirm({
        message: 'Are you sure you want to update driver information?',
        header: 'Confirm',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {               
          this.manageQueueService.updateDriverInfo(this.data)
          .pipe(untilDestroyed(this))
          .subscribe({next: (d)=>{          
            this.messageService.add({key:'dg1', severity:'success', summary: 'Successful', detail: 'Update driver information success.', life: 3000});
          },error: (error)=>{
            error.Messages.forEach((msg: any) => {
              this.messageService.add({key:'dg1', severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
          }});
        }
      });   
    }
  }

  close(): void {
    this.ref.close();
  }
}
