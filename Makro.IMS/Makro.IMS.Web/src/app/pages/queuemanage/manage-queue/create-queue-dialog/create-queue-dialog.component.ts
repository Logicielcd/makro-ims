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
import { Message, MessageService } from 'primeng/api';
import { ManageQueueService } from '@core/services/queuemanage/manage-queue.service';
import { QueueActionDto } from '@core/models/queuemanage/queue-action.model';

@UntilDestroy()
@Component({  
  templateUrl: 'create-queue-dialog.component.html',
  styleUrls: ['create-queue-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class CreateQueueDialogComponent implements OnInit {

  msgs: Message[] = [];

  operationTypes: any[];
  selectedoperationtype: any;

  operations: Operation[];
  selectedOperation: string;

  queue: QueueManageDto = {} as QueueManageDto;
  doors: Door[];
  startDoor: string;
  endDoor: string;

  queueSeq: QueueSequence = {} as QueueSequence;
  lastSequence: string = '????';
  
  isManual: boolean = false;
  isDisable: boolean = false;

  queueAction: QueueActionDto = {} as QueueActionDto;

  constructor(
    public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private authService: AuthService
    ,private operationService: OperationService
    ,private messageService: MessageService
    ,private doorService: DoorService
    ,private manageQueueService: ManageQueueService
    )
  {    
    
    this.queue = config.data.queue;

    this.selectedoperationtype = this.queue.operationType;

    console.log('queue data:',this.queue);

    this.operationService.getByWarehouse(this.queue.warehouseCode)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      this.operations = data;      
      this.selectedOperation = this.operations.find(x=>x.operationName == this.queue.operationType).operationName;
      
      this.doorService.getByWarehouse(this.queue.warehouseCode)
      .pipe(untilDestroyed(this))
      .subscribe({next: (data) => {
          this.doors = data;
          this.onOperationTypeChange();
        },
        error: (error)=>{
          this.msgs = [];
          error.Messages.forEach((msg: any) => {
          this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
          });
        }
        });
        
      },
      error: (error) => {
        this.msgs = [];
        error.Messages.forEach((msg: any) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
        });
      }      
    });
  }

  ngOnInit()
  {    
    this.isDisable = false;
  }

  confirm(){
    this.queueAction.internalHeaderKey = this.queue.internalHeaderKey;
    this.queueAction.queueNo = this.lastSequence;//this.queueSeq.lastSequence.toString();
    this.queueAction.internalTruckCheckInId = this.queue.internalTruckCheckInId;
    this.queueAction.remark = this.selectedOperation;
    this.manageQueueService.createQueue(this.queueAction)
    .pipe(untilDestroyed(this))
    .subscribe({next : (data) => {
      if(data == true){
        //let result = {result:true};

        this.isDisable = true;
        //this.ref.close(result);
      }
    },
    error: (error) => {
      this.msgs = [];
        error.Messages.forEach((msg: any) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
        });
    }});

    
  }

  onOperationTypeChange(){
    let whDoors = this.doors.filter(x=>x.doorArea == this.selectedOperation);
    this.startDoor = whDoors.sort((a,b) => a.sequence.localeCompare(b.sequence))[0].doorName;    
    this.endDoor = whDoors.sort((a,b) => b.sequence.localeCompare(a.sequence))[0].doorName;
    
  }

  onSystemGenerate(){
    this.isManual = false;
    this.manageQueueService.getQueueSequence(this.queue.warehouseCode,this.selectedOperation)
    .pipe(untilDestroyed(this))
    .subscribe({next : (data) => {      
      this.queueSeq = data;
      this.lastSequence = data.lastSequence.toString();      
      this.confirm();
    },
    error : (error) => {
      this.msgs = [];
        error.Messages.forEach((msg: any) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
        });
    }});
  }
  
  onManualGenerate(){
    this.isManual = true;
    this.lastSequence = '';
  }
}
