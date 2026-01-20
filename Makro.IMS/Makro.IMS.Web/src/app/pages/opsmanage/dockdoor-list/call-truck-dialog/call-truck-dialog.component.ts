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
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { ManageQueueService } from '@core/services/queuemanage/manage-queue.service';
import { RowGroupHeader } from 'primeng/table';
import { QueueActionDto } from '@core/models/queuemanage/queue-action.model';

@UntilDestroy()
@Component({  
  templateUrl: 'call-truck-dialog.component.html',
  styleUrls: ['call-truck-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class CallTruckDialogComponent implements OnInit {

  msgs: Message[] = [];
  
  cols: any[];

  datasource: QueueManageDto[];

  operationTypes: any[];
  selectedoperationtype: any;

  operations: Operation[];
  selectedOperation: string;

  doorQueue: DoorQueueDto = {} as DoorQueueDto;
  doors: Door[];
  startDoor: string;
  endDoor: string;

  warehouseCode: string;
  operationType: string;
 
  isLoading: boolean;

  constructor(
    public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private authService: AuthService
    ,private manageQueueService: ManageQueueService
    ,private messageService: MessageService
    ,private doorService: DoorService
    ,private confirmationService: ConfirmationService
    )
  {    

    this.doorQueue = config.data.doorQueue;
    this.warehouseCode = config.data.warehouseCode;
    this.operationType = config.data.operationType;
    this.isLoading = false;

    this.cols = [
      {
        field: 'bookingId',
        display: 'string',
        filter: 'string',
        header: 'Label.BookingId',
      },
      {
        field: 'backHaul',
        display: 'string',
        filter: 'string',
        header: 'Back Haul',
      },
      {
        field: 'operationType',
        display: 'string',
        filter: 'string',
        header: 'Label.MerchType',
      }, 
      {
        field: 'truck',
        display: 'string',
        filter: 'string',
        header: 'Label.Truck',
      },            
      {
        field: 'displaySupplier',
        display: 'string',
        filter: 'string',
        header: 'Label.SupCode',
      },
      {
        field: 'queueSeq',
        display: 'string',
        filter: 'string',
        header: 'Label.QueueSeq',
      },
      {
        field: 'waitingTimeDisplay',
        display: 'string',
        filter: 'string',
        header: 'Label.WaitingTime',
      },
      
    ];

    let search: QueueManageSearchDto = {} as QueueManageSearchDto;
    search.warehouseCode = this.warehouseCode;
    //search.operationType = this.operationType;
    search.operationType = this.doorQueue.doorArea;
    search.truckType = this.doorQueue.truckType;

    console.log(this.doorQueue);

    this.manageQueueService.getTruckQueue(search)
    .pipe(untilDestroyed(this))
    .subscribe({next : (data) => {
      this.datasource = data;
      this.datasource.forEach(row => {
        row.displaySupplier = row.supCode + '\r\n' + row.supName;
        row.truck = row.truck + '\r\n' + row.licensePlate + '\r\n' + row.licensePlate2;

        const hours = Math.floor(row.waitingTime / 3600); // คำนวณจำนวนชั่วโมง (เฉพาะชั่วโมงเต็ม)
        const minutes = Math.floor((row.waitingTime % 3600) / 60); 

        row.waitingTimeDisplay = hours.toString().padStart(2,'0') + ':' + minutes.toString().padStart(2,'0');
      });
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

  onOperationTypeChange(){
    let whDoors = this.doors.filter(x=>x.doorArea == this.selectedOperation);
    this.startDoor = whDoors.sort((a,b) => a.sequence.localeCompare(b.sequence))[0].doorName;
    // whDoors.reduce((minSeq, whDoor) => Number(whDoor.sequence) < Number(minSeq.sequence) ? whDoor : minSeq).doorName;
    this.endDoor = whDoors.sort((a,b) => b.sequence.localeCompare(a.sequence))[0].doorName;
    //whDoors.reduce((maxSeq, whDoor) => Number(whDoor.sequence) > Number(maxSeq.sequence) ? whDoor : maxSeq).doorName;
  }

  onManageQueue(rowData: any){
    this.isLoading = true;
    this.confirmationService.confirm({
      message: 'กรุณายืนยันการเรียกรถทะเบียน ' + rowData.licensePlate,
      header: 'ยืนยัน',      
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      icon: 'pi pi-info-circle',
      accept: () => {

        let queueAction : QueueActionDto = {} as QueueActionDto;
        
        queueAction.internalHeaderKey = rowData.internalHeaderKey;
        queueAction.internalDoorId = this.doorQueue.internalDoorId;
        queueAction.queueNo = rowData.queueSeq;
        queueAction.internalTruckCheckInId = rowData.internalTruckCheckInId;

        this.manageQueueService.assignDoor(queueAction)
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
    });
  }
}
