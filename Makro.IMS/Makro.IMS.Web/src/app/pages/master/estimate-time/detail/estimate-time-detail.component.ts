import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Warehouse } from '@core/models/master/warehouse.model';
import { Door } from '@core/models/master/door.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { DoorService } from '@core/services/master/door.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { OperationService } from '@core/services/master/operation.service';
import { Operation } from '@core/models/master/operation.model';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { PagingService } from '@core/services/paging.service';
import { OperationTime } from '@core/models/master/operation-time.model';

@UntilDestroy()
@Component({
  selector: 'app-estimate-time-detail',
  templateUrl: './estimate-time-detail.component.html',
})
export class EstimateTimeDetailComponent implements OnInit {
  msgs: Message[] = [];

  dataId: string;
  data: Operation = {} as Operation;

  // warehouse variable
  warehouses: Warehouse[];
  warehouseSelected: string;

  isLoading: boolean = false;
  canModify = false;

  loadCompleted: boolean = false;

  cols: any[];
  operationTimes: OperationTime[];
  visible: boolean;

  startTime: Date;
  endTime: Date;

  @ViewChild('f') f: NgForm;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    Validation: 'Message.Validation',
    CreateSuccess: 'Message.Notification.Create',
    EditSuccess: 'Message.Notification.Update',
    DeleteSuccess: 'Message.Notification.Delete',
    DeleteTitle: 'Message.Confirm.Delete.Title',
    DeleteMessage: 'Message.Confirm.Delete.Message',
    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };

  constructor(
    public route: ActivatedRoute,    
    private router: Router,
    private warehouseService: WarehouseService,    
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private authService: AuthService,
    private _location: Location,    
    private operationService: OperationService,
    private pagingService: PagingService,
  ) {

    this.visible = false;
    this.canModify = true;
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.warehousecapacity)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));

    this.canModify = true;

    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
      
  }

  ngOnInit() {
    this.isLoading = true;
    this.visible = false;
    this.cols = [
      {
        field: 'startTime',
        display: 'time',        
        header: 'Start Time',
        width: '150px',
      },      
      {
        field: 'endTime',
        display: 'time',
        header: 'End Time',
      },            
    ];

    if(this.route.snapshot.params.id == undefined){
      this.data = {} as Operation;
      this.warehouseService.getAll()
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.warehouses = data;    
          this.loadCompleted = true;
          this.isLoading = false;
        }
      });
    }
    else{
      this.warehouseService.getAll()
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.warehouses = data;      
          const decodedId = atob(this.route.snapshot.params.id);
          this.dataId = decodedId;
          if (this.dataId) {
            this.fetchData();
          }
          else{            
            this.loadCompleted = true;
            this.isLoading = false;
          }
        },
        error: (error) => {
          this.msgs = [];
            error.Messages.forEach((msg: any) => {
              this.msgs.push({
                severity: 'error',
                summary: 'Error',
                detail: this.translateService.instant(
                  `${this.translatePrefix.FromApi}.${msg}`
                ),
              });
            });
            this.isLoading = false;
        },
      });
    }
  }

  fetchData() {
    this.msgs = [];
    this.isLoading = true;

    const whseCode = this.dataId.split('|')[0];
    const opName = this.dataId.split('|')[1];

    this.operationService
      .getByOperationNameAndWhse(opName,whseCode)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.data = data;
          this.warehouseSelected = this.data.warehouseCode;

          this.operationService.getOperationTimeByWhseAndOperation(this.data.warehouseCode,this.data.operationName)
          .pipe(untilDestroyed(this))
          .subscribe((d)=> {
            this.operationTimes = d;
          });

          this.loadCompleted = true;          
          this.isLoading = false;
        },
        error: (error) => {
          this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.msgs.push({
              severity: 'error',
              summary: 'Error',
              detail: this.translateService.instant(
                `${this.translatePrefix.FromApi}.${msg}`
              ),
            });
          });
          this.isLoading = false;
        },
      });
  }

  onWarehouseChange(event:any){
    this.warehouseSelected = event.value;
    this.data.warehouseCode = this.warehouseSelected;  
  }

  onSubmit() {
    this.msgs = [];
    if (!this.dataId) {
      this.onCreate();
    } else {
      this.onUpdate();
    }
  }

  onCreate() {    
    this.isLoading = true;
    
    this.operationService.add(this.data)
    .pipe(untilDestroyed(this))
    .subscribe({next:(d)=>{
      this.messageService.add({
        severity: 'success',
        summary: 'Successful',
        detail: this.translateService.instant(
          `${this.translatePrefix.CreateSuccess}`
        ),
        life: 3000,
      });
      this.isLoading = false;
    },error:(error)=>{
      this.isLoading = false;
    }});
    
  }

  onUpdate() {
    this.isLoading = true;

    this.operationService.update(this.data)
    .pipe(untilDestroyed(this))
    .subscribe({next:(d)=>{
      this.messageService.add({
        severity: 'success',
        summary: 'Successful',
        detail: this.translateService.instant(`${this.translatePrefix.EditSuccess}`),
        life: 3000,
      });
      this.isLoading = false;
    },error: (error)=>{
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
        this.msgs.push({
          severity: 'error',
          summary: 'Error',
          detail: this.translateService.instant(
            `${this.translatePrefix.FromApi}.${msg}`
          ),
        });
      });
      this.isLoading = false;
    }});
    
  }

  onDelete() {
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.DeleteMessage
      ),
      header: this.translateService.instant(this.translatePrefix.DeleteTitle),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.confirmDelete();
      },
    });
  }

  confirmDelete() {
    this.msgs = [];
    this.isLoading = true;

    this.operationService
      .delete(this.data)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.DeleteSuccess}`
            ),
            life: 3000,
          });
          this.isLoading = false;
          this.onBack();
        },
        error: (error) => {
          this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.msgs.push({
              severity: 'error',
              summary: 'Error',
              detail: this.translateService.instant(
                `${this.translatePrefix.FromApi}.${msg}`
              ),
            });
          });
          this.isLoading = false;
        },
      });
      
  }

  onBack() {
    this._location.back();
  }
  
  onDeleteTime(row:OperationTime){
    
    this.isLoading = true;
    this.confirmationService.confirm({
      message: this.translateService.instant(
        this.translatePrefix.DeleteMessage
      ),
      header: this.translateService.instant(this.translatePrefix.DeleteTitle),
      acceptLabel: this.translateService.instant(
        this.translatePrefix.YesButton
      ),
      rejectLabel: this.translateService.instant(this.translatePrefix.NoButton),
      icon: 'pi pi-info-circle',
      accept: () => {
        console.log(row);
        this.operationService.deleteOperationTime(row)
        .pipe(untilDestroyed(this))
        .subscribe({next:(d)=>{
          this.fetchData();
          this.isLoading = false;
        },error: (error)=>{
          this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.msgs.push({
              severity: 'error',
              summary: 'Error',
              detail: this.translateService.instant(
                `${this.translatePrefix.FromApi}.${msg}`
              ),
            });
          });
          this.isLoading = false;
        }});
      },
    });
  }

  onAddTime(){
    this.visible = true;
  }

  onTimeSave(){    
    this.isLoading = true;
    let operationTime = {} as OperationTime;
    operationTime.warehouseCode = this.data.warehouseCode;
    operationTime.operationType = this.data.operationName;
    operationTime.startTime = this.startTime;
    operationTime.endTime = this.endTime;
    this.operationService.addOperationTime(operationTime)
    .pipe(untilDestroyed(this))
    .subscribe({ next: (d) =>{
      this.visible = false;
      this.fetchData();
    },error:(error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
        this.msgs.push({
          severity: 'error',
          summary: 'Error',
          detail: this.translateService.instant(
            `${this.translatePrefix.FromApi}.${msg}`
          ),
        });
      });
      this.isLoading = false;
      this.visible = false;
    }});
  }

}
