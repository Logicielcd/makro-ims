import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { Supplier } from '@core/models/master/supplier.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { DoorQueueDto, QueueManageDto, QueueManageSearchDto } from '@core/models/queuemanage/manage-queue.model';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { LanguageService } from '@core/services/language.service';
import { SupplierService } from '@core/services/master/supplier.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { PagingService } from '@core/services/paging.service';
import { ManageQueueService } from '@core/services/queuemanage/manage-queue.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { TablePageListComponent } from '@theme/components/table-page-list/table-page-list.component';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { switchMap, tap } from 'rxjs';
import { CallTruckDialogComponent } from './call-truck-dialog/call-truck-dialog.component';
import { OperationService } from '@core/services/master/operation.service';
import { QueueActionDto } from '@core/models/queuemanage/queue-action.model';
import { StatusModalComponent } from '@theme/components/modal/status-modal/status-modal.component';
import { Operation } from '@core/models/master/operation.model';
import { User } from '@core/models/account/user.model';
import { MoveTruckDialogComponent } from './move-truck-dialog/move-truck-dialog.component';
import { CancelTruckDialogComponent } from './cancel-truck-dialog/cancel-truck-dialog.component';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { TruckMaster } from '@core/models/master/truck-master.model';

@UntilDestroy()
@Component({
  templateUrl: './dockdoor-list.component.html',
})
export class DockDoorListComponent implements OnInit, OnDestroy {
  warehouses: Warehouse[] = [];
  selectedWhse: any;

  cols: any[];
  dataSource: DoorQueueDto[] = [];
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  fromDate: Date;
  toDate: Date;

  statuses: any[];
  selectedstatus: string;

  operationtypes: any[];
  selectedoperationtype: string;

  pending4W: number = 0;
  pending6W: number = 0;

  pendingTruck: any[];

  user: User;

  truckMasters: TruckMaster[];

  // supplier variable
  suppliers: Supplier[];

  ref: DynamicDialogRef;

  @ViewChild(TablePageListComponent) tableComp: TablePageListComponent;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    UploadSuccess: 'Message.Notification.Upload',
  };

  constructor(
    private truckService: TruckMasterService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private pagingService: PagingService,
    private translateService: TranslateService,
    private languageService: LanguageService,
    private authService: AuthService,
    private warehouseService: WarehouseService,
    private managequeueService: ManageQueueService,
    private operationService: OperationService
  ) {
    this.authService
      .canModify(APP_MENU.opsmanage.module, APP_MENU.opsmanage.dockdoorList)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  
    this.user = this.authService.getUser();
    
    this.languageService.language$
    .pipe(untilDestroyed(this))
    .subscribe((language: string) => {
      this.translateService.use(language);
    });
  }

  ngOnInit() {

    this.truckService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe({next:(data) =>
      this.truckMasters = data
    });

    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      this.warehouses = data;
      if(this.user.warehouseCode !== undefined && this.user.warehouseCode !== null){
        this.selectedWhse = this.user.warehouseCode;

        this.operationService.getByWarehouse(this.selectedWhse)
        .pipe(untilDestroyed(this))
        .subscribe({next: (op) => {
          let allOperation: Operation =  {} as Operation;
          this.operationtypes = op;
          allOperation.description = "ALL";
          allOperation.operationName = "ALL";
          allOperation.warehouseCode = this.selectedWhse;
          this.operationtypes.unshift(allOperation);
          
          if(this.user.operationType !== undefined && this.user.operationType !== null){
            this.selectedoperationtype = this.user.operationType;            
            this.getDoorListByOperation();
          }
          else{
            this.selectedoperationtype = "ALL";
            this.getDoorList();
          }
        }});
      }
    },
    error: (error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }});

    this.cols = [
      {
        field: 'doorName',
        display: 'string',
        filter: 'string',
        header: 'Label.DoorName',
        width: '80px',
      },
      {
        field: 'areaTruck',
        display: 'string',
        filter: 'string',
        header: 'Label.AreaTruck',
        width: '100px',
      },
      {
        field: 'status',
        display: 'string',
        filter: 'string',
        header: 'Label.Status',
        width: '150px',
      },     
      {
        field: 'waitingTimeDisplay',
        display: 'string',
        filter: 'string',
        header: 'Label.WaitingTime',
        width: '80px',
      },      
      {
        field: 'bookingSup',
        display: 'string',
        filter: 'string',
        header: 'Label.BookingSup',        
      },
      {
        field: 'licensePlate',
        display: 'string',
        filter: 'string',
        header: 'License Plate',        
      }
         
    ];

    this.fromDate = new Date();
    this.toDate = new Date();
    this.selectedstatus = null;
  }

  ngOnDestroy() {
    if (this.ref) {
      this.ref.close();
    }
  }

  refresh(): void {
    window.location.reload();
  }

  onWhseChange(){    
    this.getDoorList();
  }

  onOperationChange(){    
    if(this.selectedoperationtype === null || this.selectedoperationtype === undefined || this.selectedoperationtype === "ALL"){
      this.getDoorList();
    }
    else{
      this.getDoorListByOperation();
    }
  }

  getDoorList(){
    
    this.managequeueService.getDoorQueue(this.selectedWhse)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      
      this.dataSource = data;
      this.dataSource.forEach(row => {
        row.areaTruck = row.doorArea + '\r\n' + row.truckType;
        if(row.bookingId !== null){
          row.bookingSup = row.bookingId + '\r\n' + row.supCode + '\r\n' + row.supName;
          row.licensePlate = row.licensePlate + '\r\n' + row.bookingTruckType;

          row.waitingTimeDisplay = Math.floor(row.waitingTime/3600).toString().padStart(2,'0') + ':' + Math.floor(((row.waitingTime%3600)/60)).toString().padStart(2,'0');
        }
        else{
          row.bookingSup = null;
        }
        
        if(row.calltruckTime === null){
          row.status = 'Available';
        }

        if(row.calltruckTime !== null){
          row.status = 'Notified to truck';
        }

        if(row.onDockTime !== null){
          row.status = 'On dock';
        }

        if(row.startUnloadTime !== null){
          row.status = 'Start unload';
        }

        if(row.finishUnloadTime !== null){
          row.status = 'End unload';
        }

      });

      this.operationService.getByWarehouse(this.selectedWhse)
      .pipe(untilDestroyed(this))
      .subscribe({next: (op) => {
        let allOperation: Operation =  {} as Operation;
        this.operationtypes = op;
        allOperation.description = "ALL";
        allOperation.operationName = "ALL";
        allOperation.warehouseCode = this.selectedWhse;
        this.operationtypes.unshift(allOperation);
      }});

      
      let search: QueueManageSearchDto = {} as QueueManageSearchDto;

      search.warehouseCode = this.selectedWhse;          
      search.status = 'QUEUE';
      if(this.selectedoperationtype !== undefined && this.selectedoperationtype !== null){
        search.operationType = this.selectedoperationtype;
      }
      
      this.managequeueService.getManageQueue(search)
      .pipe(untilDestroyed(this))
      .subscribe({next: (queue) => {        
        this.calculateTruckPending(queue);        
      }});
    },
    error: (error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }});
  }

  getDoorListByOperation(){
    this.managequeueService.getDoorQueueByOperation(this.selectedWhse,this.selectedoperationtype)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      this.dataSource = data;      
      this.dataSource.forEach(row => {
        row.areaTruck = row.doorArea + '\r\n' + row.truckType;
        if(row.bookingId !== null){
          row.bookingSup = row.bookingId + '\r\n' + row.supCode + '\r\n' + row.supName;
          row.licensePlate = row.licensePlate + '\r\n' + row.bookingTruckType;
          row.waitingTimeDisplay = Math.floor(row.waitingTime/3600).toString().padStart(2,'0') + ':' + Math.floor(((row.waitingTime%3600)/60)).toString().padStart(2,'0');
        }
        else{
          row.bookingSup = null;
        }
        
        if(row.calltruckTime === null){
          row.status = 'Available';
        }

        if(row.calltruckTime !== null){
          row.status = 'Notified to truck';
        }

        if(row.onDockTime !== null){
          row.status = 'On dock';
        }

        if(row.startUnloadTime !== null){
          row.status = 'Start unload';
        }

        if(row.finishUnloadTime !== null){
          row.status = 'End unload';
        }

      });
      
      let search: QueueManageSearchDto = {} as QueueManageSearchDto;

      search.warehouseCode = this.selectedWhse;          
      search.status = 'QUEUE';
      if(this.selectedoperationtype !== undefined && this.selectedoperationtype !== null){
        search.operationType = this.selectedoperationtype;
      }

      this.managequeueService.getManageQueue(search)
      .pipe(untilDestroyed(this))
      .subscribe({next: (queue) => {        
        this.calculateTruckPending(queue);
        // this.pending4W = queue.filter(x=>x.truck == '4 W').length;
        // this.pending6W = queue.filter(x=>x.truck !== '4 W').length;
      }});
    },
    error: (error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }});
  }

  onManageQueue(queue: DoorQueueDto){
   
  }

  calculateTruckPending(queue: any){
    
    const queuePendingTruck = Object.entries(
      queue.reduce((acc, item) => {
          if (acc[item.truck]) {
              acc[item.truck] += 1;
          } else {
              acc[item.truck] = 1;
          }
          return acc;
      }, {} as Record<string, number>)
    ).map(([truckType, qty]) => ({ truckType, qty }));

    // รวมข้อมูลจาก truckMaster
    this.pendingTruck = this.truckMasters.map(master => {
      const pending = queuePendingTruck.find(p => p.truckType === master.truckCode);
      return {
        truckType: master.truckCode,        
        qty: pending ? pending.qty : 0, 
      };
    });

  }

  onCallTruck(doorQueue: DoorQueueDto){
    this.ref = this.dialogService.open(CallTruckDialogComponent, {
      header: 'Call Truck',
      showHeader: true,
      width: '80vw',
      contentStyle: {"max-height": "1600", "min-height" : "600px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        doorQueue: doorQueue,
        warehouseCode: this.selectedWhse,
        operationType: this.selectedoperationtype
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{
      if (ret.result == true)
      {
        if(this.selectedoperationtype !== 'ALL'){
          this.onOperationChange();
        }
        else{
          this.onWhseChange();
        }
      }
    });
  }

  onTracking(doorQueue: DoorQueueDto){
    let queue : QueueManageDto = {} as QueueManageDto;

    queue.bookingId = doorQueue.bookingId;
    queue.internalTruckCheckInId = doorQueue.internalTruckCheckInId;

    this.ref = this.dialogService.open(StatusModalComponent,{
      header: 'Status',
      width: '1200px',
      contentStyle: {"max-height": "1600", "min-height" : "600px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        queue: queue,
      }
    });
  }

  onUpdate(queue: DoorQueueDto){
    let status = '';
    let msg = ''

    if(queue.calltruckTime !== null && queue.onDockTime == null){
      status = 'calltruck';
      msg = 'นำรถเข้าประตู';
    }
    if(queue.onDockTime !== null && queue.startUnloadTime == null){
      status = 'start';
      msg = 'เริ่ม unload สินค้า';
    }
    if(queue.startUnloadTime !== null && queue.finishUnloadTime == null){
      status = 'finish';
      msg = 'เสร็จสิ้นการ unload สินค้า';
    }
    if(queue.startUnloadTime !== null && queue.finishUnloadTime !== null){
      status = 'leave';
      msg = 'นำรถออกจากประตู';
    }
    
    this.confirmationService.confirm({
      message: 'กรุณายืนยันการ' + msg,
      header: 'ยืนยัน',      
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      icon: 'pi pi-info-circle',
      accept: () => {
        let queueAction : QueueActionDto = {} as QueueActionDto;
        queueAction.internalDoorId = queue.internalDoorId;
        queueAction.internalHeaderKey = queue.bookingHeaderKey;
        queueAction.internalTruckCheckInId = queue.internalTruckCheckInId;
        queueAction.queueNo = '0';

        if(status == 'calltruck'){
          this.managequeueService.truckOnDoor(queueAction)
          .pipe(untilDestroyed(this))
          .subscribe({next:(data)=>{
            if(data === true){
              this.messageService.add({
                severity: 'success',
                summary: 'Successful',
                detail: 'Update สถานะ เรียบร้อย',
                life: 3000,
              })
              if(this.selectedoperationtype !== 'ALL'){
                this.onOperationChange();
              }
              else{
                this.onWhseChange();
              }
            }
          },
          error:(error)=>{
            this.msgs = [];
              error.Messages.forEach((msg: any) => {
                this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
              });       
          }});
        }

        if(status == 'start'){
          this.managequeueService.startUnloading(queueAction)
          .pipe(untilDestroyed(this))
          .subscribe({next:(data)=>{
            if(data === true){
              this.messageService.add({
                severity: 'success',
                summary: 'Successful',
                detail: 'Update สถานะ เรียบร้อย',
                life: 3000,
              })
              if(this.selectedoperationtype !== 'ALL'){
                this.onOperationChange();
              }
              else{
                this.onWhseChange();
              }
            }
          },
          error:(error)=>{
            this.msgs = [];
              error.Messages.forEach((msg: any) => {
                this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
              });       
          }});
        }

        if(status == 'finish'){
          this.managequeueService.finishUnloading(queueAction)
          .pipe(untilDestroyed(this))
          .subscribe({next:(data)=>{
            if(data === true){
              this.messageService.add({
                severity: 'success',
                summary: 'Successful',
                detail: 'Update สถานะ เรียบร้อย',
                life: 3000,
              })
              if(this.selectedoperationtype !== 'ALL'){
                this.onOperationChange();
              }
              else{
                this.onWhseChange();
              }
            }
          },
          error:(error)=>{
            this.msgs = [];
              error.Messages.forEach((msg: any) => {
                this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
              });       
          }});
        }

        if(status == 'leave'){
          this.managequeueService.leaveDoor(queueAction)
          .pipe(untilDestroyed(this))
          .subscribe({next:(data)=>{
            if(data === true){
              this.messageService.add({
                severity: 'success',
                summary: 'Successful',
                detail: 'Update สถานะ เรียบร้อย',
                life: 3000,
              })
              if(this.selectedoperationtype !== 'ALL'){
                this.onOperationChange();
              }
              else{
                this.onWhseChange();
              }
            }
          },
          error:(error)=>{
            this.msgs = [];
              error.Messages.forEach((msg: any) => {
                this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
              });       
          }});
        }

      },
    });
  }

  onMoveTruck(doorQueue: DoorQueueDto){
    this.ref = this.dialogService.open(MoveTruckDialogComponent, {
      header: 'Move Truck',
      showHeader: true,
      width: '600px',
      contentStyle: {"max-height": "1600", "min-height" : "400px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        doorQueue: doorQueue,
        warehouseCode: this.selectedWhse,
        operationType: this.selectedoperationtype
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{
      if (ret.result == true)
      {
        if(this.selectedoperationtype !== 'ALL'){
          this.onOperationChange();
        }
        else{
          this.onWhseChange();
        }
      }
    });
  }

  getWaitingTimeFontColor(fieldData:string){
    if(fieldData !== undefined){      
      if( +fieldData.split(':')[1] > 30 ){
        return 'red'
      }
      else
      {
        return 'var(--text-color)'
      }
    }
    else{
      return 'var(--text-color)'
    }    
  }

  onCancelTruck(doorQueue: DoorQueueDto){
    this.ref = this.dialogService.open(CancelTruckDialogComponent, {
      header: 'Change Truck',
      showHeader: true,
      width: '600px',
      contentStyle: {"max-height": "1200", "min-height" : "300px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        doorQueue: doorQueue,
        warehouseCode: this.selectedWhse,
        operationType: this.selectedoperationtype
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{
      if (ret.result == true)
      {
        if(this.selectedoperationtype !== 'ALL'){
          this.onOperationChange();
        }
        else{
          this.onWhseChange();
        }
      }
    });
  }


}
