import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { Supplier } from '@core/models/master/supplier.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { QueueManageDto, QueueManageSearchDto } from '@core/models/queuemanage/manage-queue.model';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { LanguageService } from '@core/services/language.service';
import { PagingService } from '@core/services/paging.service';
import { ManageQueueService } from '@core/services/queuemanage/manage-queue.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { TablePageListComponent } from '@theme/components/table-page-list/table-page-list.component';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { switchMap, tap } from 'rxjs';
import { CreateQueueDialogComponent } from '../create-queue-dialog/create-queue-dialog.component';
import { StatusModalComponent } from '@theme/components/modal/status-modal/status-modal.component';
import { Warehouse } from '@core/models/master/warehouse.model';
import { SendDocumentDialogComponent } from '../send-document-dialog/send-document-dialog.component';
import { QueueActionDto } from '@core/models/queuemanage/queue-action.model';
import { Operation } from '@core/models/master/operation.model';
import { OperationService } from '@core/services/master/operation.service';
import { User } from '@core/models/account/user.model';
import { DocumentCheckingDialogComponent } from '../document-checking-dialog/document-checking-dialog.component';
import { DocumentConfirmationDialogComponent } from '../document-confirmation-dialog/document-confirmation-dialog.component';
import { UnloadFinishDialogComponent } from '../unload-finish-dialog/unload-finish-dialog.component';
import { CancelPoDialogComponent } from '../cancel-po-dialog/cancel-po-dialog.component';
import { DriverDialogComponent } from '../driver-dialog/driver-dialog.component';

@UntilDestroy()
@Component({
  templateUrl: './manage-queue-list.component.html',
})
export class ManageQueueListComponent implements OnInit, OnDestroy {
  cols: any[];
  dataSource: QueueManageDto[] = [];
  rowCount: number = 0;
  msgs: Message[] = [];
  canModify = false;

  isCompleted = false;

  fromDate: Date;
  toDate: Date;

  warehouses: Warehouse[] = [];
  selectedWhse: any;

  statuses: any[];
  selectedstatus: string;

  operationtypes: any[];
  selectedoperationtype: string;

  // supplier variable
  suppliers: Supplier[];
  user: User;

  ref: DynamicDialogRef;

  bookingId: string = "";
  licensePlate: string = "";

  onlyView: boolean;

  @ViewChild(TablePageListComponent) tableComp: TablePageListComponent;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    UploadSuccess: 'Message.Notification.Upload',
  };

  constructor(
    private bookingHeaderService: BookingHeaderService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private pagingService: PagingService,
    private translateService: TranslateService,
    private languageService: LanguageService,
    private authService: AuthService,
    private warehouseService: WarehouseService,
    private managequeueService: ManageQueueService,
    private operationService: OperationService,
  ) {
    this.authService
      .canModify(APP_MENU.booking.module, APP_MENU.booking.bookingHeader)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
  
    this.user = this.authService.getUser();
    
    this.onlyView = false;

    if(this.user.userType.toUpperCase() === 'CONTROL'){
      this.onlyView = true;
    }

    this.languageService.language$
    .pipe(untilDestroyed(this))
    .subscribe((language: string) => {
      this.translateService.use(language);
    });
  }

  ngOnInit() {
    
    this.bookingId = "";
    this.licensePlate = "";
    
    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      this.warehouses = data;
      if(this.user.warehouseCode !== undefined || this.user.warehouseCode !== null){
        this.selectedWhse = this.user.warehouseCode;
        this.onWhseChange();
      }
    },
    error: (error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }});

    this.statuses = [
      {
        value: "ALL",
        display: "All status"
      },
      {
        value: "CHECKIN",
        display: "Guard Check In"
      },
      {
        value: "QUEUE",
        display: "Assign Queue"
      },
      {
        value: "CALLTRUCK",
        display: "Assign Truck"
      },
      {
        value: "ONDOCK",
        display: "On Dock"
      },
      {
        value: "UNLOADING",
        display: "Start UnLoading"
      },
      {
        value: "UNLOADED",
        display: "Finish UnLoading"
      },
      {
        value: "LEAVEDOOR",
        display: "Leave Door"
      },
      {
        value: "WAITINGDOC",
        display: "Waiting document"
      },
      {
        value: "SUBMITDOC",
        display: "Confirm document"
      },
      {
        value: "CHECKOUT",
        display: "Guard Check Out"
      }
    ];

    this.cols = [      
      {
        field: 'bookingIdDisplay',
        display: 'string',
        filter: 'string',
        header: 'Label.BookingId',        
        align: 'left',
        width: '150px',
      },
      {
        field: 'statusDisplay',
        display: 'string',        
        header: 'Label.Status',        
        align: 'center',
      },
      {
        field: 'operationType',
        display: 'string',
        filter: 'string',
        header: 'Label.MerchType',        
        align: 'center',
        width: '70px',
      },
      {
        field: 'truck',
        display: 'string',
        filter: 'string',
        header: 'Label.Truck',
        // width: '80px',
      },
      {
        field: 'lastUpDate',
        display: 'datetime',
        filter: 'string',
        header: 'Label.LastUpDate',
        width: '120px',
      },
      {
        field: 'backHaul',
        display: 'string',
        align: 'center',
        header: 'BH',
        width: '70px',
      },
      {
        field: 'waitingTimeDisplay',
        display: 'string',
        header: 'Waiting Time',
        width: '80px',
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
        width: '50px',
      },    
    ];

    this.fromDate = new Date();
    this.toDate = new Date();
    this.selectedstatus = null;

  }

  getWaitingTimeFontColor(fieldData:string){
    if( +fieldData.split(':')[0] > 0 ){
      return 'red'
    }
    else
    {
      return 'var(--text-color)'
    }
  }

  ngOnDestroy() {
    if (this.ref) {
      this.ref.close();
    }
  }

  refresh(): void {
    window.location.reload();
  }

  onView(){
    let search: QueueManageSearchDto = {} as QueueManageSearchDto;

    search.warehouseCode = this.selectedWhse;
    search.startDate = this.fromDate;
    search.endDate = this.toDate;
    search.status = this.selectedstatus;
    search.operationType = this.selectedoperationtype;
    search.bookingId = this.bookingId;
    search.licensePlate = this.licensePlate;

    this.managequeueService.getManageQueue(search)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      this.dataSource = data;
      this.dataSource.forEach(row => {        
        row.bookingIdDisplay = row.bookingId + '\r\n' + '(' + row.totalTruck + ')';
        row.displaySupplier = row.supCode + '\r\n' + row.supName;
        row.truck = row.truck + '\r\n' + row.licensePlate + "\r\n" + row.licensePlate2;
        row.waitingTimeDisplay = Math.floor(row.waitingTime/3600).toString().padStart(2,'0') + ':' + Math.floor(((row.waitingTime%3600)/60)).toString().padStart(2,'0');
        row.isFinish = row.status === 'LEAVEDOOR' ? true : false;
        row.statusDisplay = this.statuses.find(x=>x.value === row.status).display;        
      });
      
    },
    error: (error) => {
      this.msgs = [];
            error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
    }});
  }

  onManageQueue(queue: QueueManageDto){
    this.ref = this.dialogService.open(CreateQueueDialogComponent, {
      header: 'Create Queue',
      width: '600px',
      contentStyle: {"max-height": "1000", "min-height" : "500px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        queue: queue,
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{          
      this.onView();   
    });

  }
  
  onDocumentChecking(queue: QueueManageDto){
    this.ref = this.dialogService.open(DocumentCheckingDialogComponent, {
      header: 'Document Checking',
      width: '600px',
      contentStyle: {"max-height": "1000", "min-height" : "500px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        queue: queue,
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{    
      if (ret.result === true)
      {
        this.onView();       
      }
    });

  }

  onTracking(queue: QueueManageDto){
    this.ref = this.dialogService.open(StatusModalComponent,{
      header: 'Status',
      width: '1200px',
      contentStyle: {"max-height": "1000", "min-height" : "600px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        queue: queue,
      }
    });
  }

  onSendDocument(queue: QueueManageDto){
    this.confirmationService.confirm({
      message: 'กรุณายืนยันการส่งเอกสาร',
      header: 'ยืนยัน',
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      icon: 'pi pi-info-circle',
      accept: () => {
        let queueAction : QueueActionDto = {} as QueueActionDto;
        queueAction.internalDoorId = 0;
        queueAction.internalHeaderKey = queue.internalHeaderKey;
        queueAction.internalTruckCheckInId = queue.internalTruckCheckInId;
        queueAction.queueNo = '0';

        this.managequeueService.sendDocument(queueAction)
        .pipe(untilDestroyed(this))
        .subscribe({next:(data)=>{
          if(data === true){
            this.messageService.add({
              severity: 'success',
              summary: 'Successful',
              detail: 'Update สถานะ เรียบร้อย',
              life: 3000,
            })
            this.onView();
          }
        },
        error:(error)=>{
          this.msgs = [];
            error.Messages.forEach((msg: any) => {
              this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
            });       
        }});
      
      }
    });
  }

  onWhseChange(){    
    if(this.selectedWhse !== null && this.selectedWhse !== undefined)
    {
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
      this.isCompleted = true;
    }
    else{
      this.isCompleted = false;
    }
  }

  onSendSms(queue: QueueManageDto){    
    this.confirmationService.confirm({
      message: 'ต้องการส่ง sms ใช่หรือไม่',
      header: 'ยืนยัน',      
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      icon: 'pi pi-info-circle',
      accept: () => {
        this.managequeueService.sendSms(queue).pipe(untilDestroyed(this))
        .subscribe({next: (res) => {
          
        },error: (err) => {

        }});
      },
    });
  }

  onFinish(queue: QueueManageDto){
    this.ref = this.dialogService.open(UnloadFinishDialogComponent, {
      header: 'ลงสินค้าเสร็จเรียบร้อย',
      width: '600px',
      contentStyle: {"max-height": "1000", "min-height" : "500px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        queue: queue,
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{    
      this.onView();      
    });
  }

  onDocumentConfirm(queue: QueueManageDto){
    this.ref = this.dialogService.open(DocumentConfirmationDialogComponent, {
      header: 'Document Confirm',
      width: '600px',
      contentStyle: {"max-height": "1000", "min-height" : "500px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        queue: queue,
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{    
      this.onView();      
    });
  }

  onCancelPo(queue: QueueManageDto){
    this.ref = this.dialogService.open(CancelPoDialogComponent, {
      header: 'Cancel PO',
      width: '600px',
      contentStyle: {"max-height": "1000", "min-height" : "500px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        queue: queue,
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{    
      this.onView();      
    });
  }

  onDriverData(queue: QueueManageDto){
    this.ref = this.dialogService.open(DriverDialogComponent, {
      header: 'Driver information',
      width: '500px',
      contentStyle: {"max-height": "1000", "min-height" : "500px", "overflow": "auto"},        
      baseZIndex: 1000,
      data: {
        queue: queue,
      }
    });

    this.ref.onClose.subscribe((ret: any) =>{    
      this.onView();      
    });
  }
}
