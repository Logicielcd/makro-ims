import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { AuthService } from 'auth/auth.service';
import { PoService } from '@core/services/booking/po.service';
import { DoorService } from '@core/services/master/door.service';

import { User } from '@core/models/account/user.model';
import { Door } from '@core/models/master/door.model';
import { QueueManageDto, QueueSequence } from '@core/models/queuemanage/manage-queue.model';
import { Operation } from '@core/models/master/operation.model';

import { OperationService } from '@core/services/master/operation.service';
import { ConfirmationService, FilterMatchMode, Message, MessageService, SelectItem } from 'primeng/api';
import { ManageQueueService } from '@core/services/queuemanage/manage-queue.service';
import { QueueActionDto } from '@core/models/queuemanage/queue-action.model';
import { BookingCheckInDto, CheckIn, PoCheckInOut } from '@core/models/booking/booking-header.model';
import { GuardCheckInOutService } from '@core/services/booking/check-in.service';
import { PoList } from '@core/models/booking/po.model';

@UntilDestroy()
@Component({  
  templateUrl: 'cancel-po-dialog.component.html',
  styleUrls: ['cancel-po-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class CancelPoDialogComponent implements OnInit {

  user: User;
  remark: string;
  inputLabel: string;

  queue: QueueManageDto;

  cols: any[];
  poCheckIns: PoCheckInOut[];
  totalPo: number;
  remarkCancelPo: string;
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
    this.inputLabel = "Remark";
    this.user = authService.getUser();
    this.queue = config.data.queue;    
  }

  ngOnInit()
  {    
    this.remark = "";        
    this.cols = [      
      { field: 'poNbr', header: 'PO No.' },
    ];

    this.isDisable = true;

    this.manageQueueService.getPoCheckin(this.queue.internalTruckCheckInId)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data)=>{
      this.poCheckIns = data;

      this.totalPo = this.poCheckIns.length;
      
    },error: (error)=>{
      error.Messages.forEach((msg: any) => {
        this.messageService.add({key:'dg', severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }}); 
  }

  onDeletePo(rowData: any){
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete a PO?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {       
        let checkIn= {} as CheckIn;
        checkIn.poNbr = rowData.poNbr;
        checkIn.userStamp = this.user.userId;
        checkIn.supCode = this.queue.supCode;
        checkIn.bookingHeaderId = rowData.id;
        checkIn.internalTruckCheckInId = this.queue.internalTruckCheckInId;
        checkIn.bookingId = this.queue.bookingId;
        checkIn.remark = this.remarkCancelPo;
        
        this.manageQueueService.deleteUnloadPo(checkIn)
        .pipe(untilDestroyed(this))
        .subscribe({next: (d)=>{
          this.poCheckIns = d;
          this.totalPo = this.poCheckIns.length;
          this.messageService.add({key:'dg', severity:'success', summary: 'Successful', detail: 'delete PO success.', life: 3000});
        },error: (error)=>{
          error.Messages.forEach((msg: any) => {
            this.messageService.add({key:'dg', severity:'error', summary: 'Error', detail: msg, life: 3000});
          });
        }});
      }
    });   
  }

  canCancelPo():boolean{    
    if(this.remarkCancelPo !== undefined && this.remarkCancelPo.length > 0){
      return false;
    }
    else{
      return true;
    }
  }
}
