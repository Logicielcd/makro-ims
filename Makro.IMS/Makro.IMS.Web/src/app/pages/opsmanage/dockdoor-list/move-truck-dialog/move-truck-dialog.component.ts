import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { AuthService } from 'auth/auth.service';
import { PoService } from '@core/services/booking/po.service';
import { DoorService } from '@core/services/master/door.service';

import { User } from '@core/models/account/user.model';
import { Door } from '@core/models/master/door.model';
import { DoorQueueDto, QueueManageDto, QueueManageSearchDto } from '@core/models/queuemanage/manage-queue.model';
import { Operation } from '@core/models/master/operation.model';

import { OperationService } from '@core/services/master/operation.service';
import { Message, MessageService } from 'primeng/api';
import { ManageQueueService } from '@core/services/queuemanage/manage-queue.service';
import { RowGroupHeader } from 'primeng/table';
import { QueueActionDto } from '@core/models/queuemanage/queue-action.model';

@UntilDestroy()
@Component({  
  templateUrl: 'move-truck-dialog.component.html',
  styleUrls: ['move-truck-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class MoveTruckDialogComponent implements OnInit {
  msgs: Message[] = [];
  cols: any[];

  doorQueue: DoorQueueDto = {} as DoorQueueDto;
  doors: Door[];
  selectedDoor: any;

  remark: string;
  warehouseCode: string;
  operationType: string;
 
  constructor(
    public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private authService: AuthService
    ,private manageQueueService: ManageQueueService
    ,private messageService: MessageService
    ,private doorService: DoorService
    )
  {    

    this.doorQueue = config.data.doorQueue;
    this.warehouseCode = config.data.warehouseCode;
    this.operationType = config.data.operationType;

    this.doorService.getByWarehouseAndOperation(this.warehouseCode,this.operationType,this.doorQueue.internalTruckCheckInId)
    .pipe(untilDestroyed(this))
    .subscribe({next : (data) => {      
      this.doors = data;
    },
    error: (error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }});

  }

  ngOnInit()
  {    

  }

  confirm(){    
    this.ref.close();
  }

  onDoorChange(){

  }

  onConfirm(){
    let queueAction : QueueActionDto = {} as QueueActionDto;
    
    queueAction.internalHeaderKey = this.doorQueue.bookingHeaderKey;
    queueAction.internalDoorId = this.doorQueue.internalDoorId;
    queueAction.newInternalDoorId = this.selectedDoor;
    queueAction.queueNo = "";
    queueAction.internalTruckCheckInId = this.doorQueue.internalTruckCheckInId;
    queueAction.remark = this.remark;

    this.manageQueueService.changeDoor(queueAction)
    .pipe(untilDestroyed(this))
    .subscribe({next:(data)=>{
      if(data === true){
        let result = {result:true};
        this.ref.close(result);
      }
    },
    error: (error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }
    });
  }

  onCancel(){
    this.ref.close();
  }

}
