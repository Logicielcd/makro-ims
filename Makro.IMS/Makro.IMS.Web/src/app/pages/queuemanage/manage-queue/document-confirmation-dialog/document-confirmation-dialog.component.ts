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
  templateUrl: 'document-confirmation-dialog.component.html',
  styleUrls: ['document-confirmation-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class DocumentConfirmationDialogComponent implements OnInit {

  user: User;

  byPo: boolean;
  poNo: string;
  inputLabel: string;

  bookingCheckIns: BookingCheckInDto[] = [];

  queue: QueueManageDto;
  
  cols: any[];
  matchModeOptions: SelectItem[];
  pos: PoList[];
  poCheckIns: PoCheckInOut[];

  totalPo: number;
  constructor(
    public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private authService: AuthService
    ,private messageService: MessageService
    ,private guardCheckInOutService: GuardCheckInOutService
    ,private manageQueueService: ManageQueueService
    ,private confirmationService: ConfirmationService
    )
  {  
    this.inputLabel = "PO No";
    this.user = authService.getUser();
    this.queue = config.data.queue;
  }

  ngOnInit()
  {    
    this.poNo = "";
    this.byPo = true;
    this.totalPo = 0;

    this.cols = [      
      { field: 'poNbr', header: 'PO No.' },
    ];

    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS }
    ];

    // get po check in out data of this truck
    this.manageQueueService.getPoCheckout(this.queue.internalTruckCheckInId)
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

  checkPo()
  {
    let checkIn= {} as CheckIn;
    checkIn.poNbr = this.poNo;
    checkIn.userStamp = this.user.userId;
    checkIn.supCode = this.queue.supCode;
    checkIn.bookingHeaderId = this.queue.internalHeaderKey;
    checkIn.internalTruckCheckInId = this.queue.internalTruckCheckInId;
    checkIn.bookingId = this.queue.bookingId;

    this.byPo = true;

    if(this.byPo){
      this.manageQueueService.documentCheckOutPo(checkIn)
        .pipe(untilDestroyed(this))
        .subscribe({
          next: (data) =>
          {
            this.poCheckIns = data;
            this.messageService.add({key:'dg', severity:'success', summary: 'Successful', detail: 'Document confirm success.', life: 3000});
          }, error: (error) =>
          {            
            error.Messages.forEach((msg: any) => {
              this.messageService.add({key:'dg', severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
          }
        });
    }
    
    this.poNo = '';
  }

  onConfirm()
  {
    let queueAction : QueueActionDto = {} as QueueActionDto;
    queueAction.internalDoorId = 0;
    queueAction.internalHeaderKey = this.queue.internalHeaderKey;
    queueAction.internalTruckCheckInId = this.queue.internalTruckCheckInId;
    queueAction.queueNo = '0';

    this.manageQueueService.sendDocument(queueAction)
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) =>
      {            
        this.messageService.add({key:'dg', severity:'success', summary: 'Successful', detail: 'Document confirm success.', life: 3000});
        this.ref.close();
      }, error: (error) =>
      {            
        error.Messages.forEach((msg: any) => {
          this.messageService.add({key:'dg', severity:'error', summary: 'Error', detail: msg, life: 3000});
        });
      }
    });    
  }

  deletePo(rowData: any){
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
        
        this.manageQueueService.deleteCheckOutPo(checkIn)
        .pipe(untilDestroyed(this))
        .subscribe({next: (d)=>{
          this.poCheckIns = d;
          this.messageService.add({key:'dg', severity:'success', summary: 'Successful', detail: 'delete PO success.', life: 3000});
        },error: (error)=>{
          error.Messages.forEach((msg: any) => {
            this.messageService.add({key:'dg', severity:'error', summary: 'Error', detail: msg, life: 3000});
          });
        }});
      }
    });   
  }
}
