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
  templateUrl: 'send-document-dialog.component.html',
  styleUrls: ['send-document-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class SendDocumentDialogComponent implements OnInit {

  msgs: Message[] = [];

  submitDocDate: Date;

  operationTypes: any[];

  queue: QueueManageDto = {} as QueueManageDto;
  queueSeq: QueueSequence = {} as QueueSequence;
  lastSequence: number = 0;
 
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
  }

  ngOnInit()
  {    

  }

  confirm(){
    this.queueAction.internalHeaderKey = this.queue.internalHeaderKey;
    this.queueAction.queueNo = this.queueSeq.lastSequence.toString();
    this.manageQueueService.sendDocument(this.queueAction)
    .pipe(untilDestroyed(this))
    .subscribe({next : (data) => {
      if(data == true){
        let result = {result:true};
        this.ref.close(result);
      }
    },
    error: (error) => {
      this.msgs = [];
        error.Messages.forEach((msg: any) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
        });
    }});

    
  }
  
}
