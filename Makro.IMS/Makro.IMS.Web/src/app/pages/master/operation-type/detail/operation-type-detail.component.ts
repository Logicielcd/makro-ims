import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Warehouse } from '@core/models/master/warehouse.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { OperationService } from '@core/services/master/operation.service';
import { Operation } from '@core/models/master/operation.model';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { OperationTime } from '@core/models/master/operation-time.model';
import { OperationCapacity } from '@core/models/master/operation-capacity.model';
import { OperationFixSlot } from '@core/models/master/operation-fix-slot.model';
import { SupplierGroup } from '@core/models/master/supplier.model';
import { SupplierGroupService } from '@core/services/master/supplier-group.service';
import { createSubjectOnTheInstance } from '@ngneat/until-destroy/lib/internals';

@UntilDestroy()
@Component({
  selector: 'app-operation-type-detail',
  templateUrl: './operation-type-detail.component.html',
  styleUrls: ['./operation-type-detail.component.scss'],
})
export class OperationTypeDetailComponent implements OnInit {
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

  visibleFixSlot: boolean;

  startTime: Date;
  endTime: Date;

  fixStartTime: Date;
  fixEndTime: Date;

  operationCapacities: OperationCapacity[];
  capCols: any[];
  capDatas: any[];
  clonedCapData: { [s: string]: any } = {};

  operationFixSlotAll: OperationFixSlot[];
  operationFixSlots: OperationFixSlot[];
  fixSlotCols: any[];
  fixCols: any[];
  fixDatas: any[];


  supplierGroups: SupplierGroup[];
  dayOfWeeks: any[];

  // fix slot dialog
  supplierGroupSelected: string;
  dayOfWeekSelected: string;
  isVip: boolean;

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
    private warehouseService: WarehouseService,    
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private authService: AuthService,
    private _location: Location,    
    private operationService: OperationService,    
    private supplierGroupService: SupplierGroupService
  ) {

    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.operationType)
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
    this.fixStartTime = new Date();
    this.fixStartTime.setHours(0,0,0);
    this.fixEndTime = new Date();
    this.fixEndTime.setHours(0,0,0);

    this.startTime = new Date();
    this.startTime.setHours(0,0,0);
    this.endTime = new Date();
    this.endTime.setHours(0,0,0);

    this.isLoading = true;
    this.visible = false;
    this.visibleFixSlot = false;

    this.dayOfWeeks = [
      {
        value: '2',
        display: 'Monday'
      },
      {
        value: '3',
        display: 'Tuesday'
      },
      {
        value: '4',
        display: 'Wednesday'
      },
      {
        value: '5',
        display: 'Thursday'
      },
      {
        value: '6',
        display: 'Friday'
      },
      {
        value: '7',
        display: 'Saturday'
      },
      {
        value: '1',
        display: 'SunDay'
      }
    ]

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

    this.capCols = [
      {
        field: 'time',
        display: 'string',
        header: 'Time',
        width: '150px',
        frozen: true
      },
      {
        field: 'monCap',
        display: 'string',
        header: 'Monday',
        width: '150px',
        frozen: false,
        subHeader: 'Cap'
      },
      {
        field: 'monTruck',
        display: 'string',
        header: 'Monday',
        width: '150px',
        frozen: false,
        subHeader: 'Truck'
      },
      {
        field: 'tueCap',
        display: 'string',
        header: 'Tuesday',
        width: '150px',
        frozen: false,
        subHeader: 'Cap'
      },
      {
        field: 'tueTruck',
        display: 'string',
        header: 'Tuesday',
        width: '150px',
        frozen: false,
        subHeader: 'Truck'
      },
      {
        field: 'wedCap',
        display: 'string',
        header: 'Wednesday',
        width: '150px',
        frozen: false,
        subHeader: 'Cap'
      },
      {
        field: 'wedTruck',
        display: 'string',
        header: 'Wednesday',
        width: '150px',
        frozen: false,
        subHeader: 'Truck'
      },
      {
        field: 'thuCap',
        display: 'string',
        header: 'Thursday',
        width: '150px',
        frozen: false,
        subHeader: 'Cap'
      },
      {
        field: 'thuTruck',
        display: 'string',
        header: 'Thursday',
        width: '150px',
        frozen: false,
        subHeader: 'Truck'
      },
      {
        field: 'friCap',
        display: 'string',
        header: 'Friday',
        width: '150px',
        frozen: false,
        subHeader: 'Cap'
      },
      {
        field: 'friTruck',
        display: 'string',
        header: 'Friday',
        width: '150px',
        frozen: false,
        subHeader: 'Truck'
      },
      {
        field: 'satCap',
        display: 'string',
        header: 'Saturday',
        width: '150px',
        frozen: false,
        subHeader: 'Cap'
      },
      {
        field: 'satTruck',
        display: 'string',
        header: 'Saturday',
        width: '150px',
        frozen: false,
        subHeader: 'Truck'
      },
      {
        field: 'sunCap',
        display: 'string',
        header: 'Sunday',
        width: '150px',
        frozen: false,
        subHeader: 'Cap'
      },
      {
        field: 'sunTruck',
        display: 'string',
        header: 'Sunday',
        width: '150px',
        frozen: false,
        subHeader: 'Truck'
      },
    ];    

    this.fixCols = [
      {
        field: 'time',
        display: 'string',
        header: 'Time',
        width: '150px',
        frozen: true
      },
      {
        field: 'monCap',
        display: 'string',
        header: '',
        width: '150px',
        frozen: false
      },
      {
        field: 'tueCap',
        display: 'string',
        header: '',
        width: '150px',
        frozen: false
      },
      {
        field: 'wedCap',
        display: 'string',
        header: '',
        width: '150px',
        frozen: false
      },
      {
        field: 'thuCap',
        display: 'string',
        header: '',
        width: '150px',
        frozen: false
      },
      {
        field: 'friCap',
        display: 'string',
        header: '',
        width: '150px',
        frozen: false
      },
      {
        field: 'satCap',
        display: 'string',
        header: '',
        width: '150px',
        frozen: false
      },
      {
        field: 'sunCap',
        display: 'string',
        header: '',
        width: '150px',
        frozen: false
      },
      {
        field: 'monFix',
        display: 'string',
        header: 'Monday',
        width: '150px',
        frozen: false
      },
      {
        field: 'tueFix',
        display: 'string',
        header: 'Tuesday',
        width: '150px',
        frozen: false
      },
      {
        field: 'wedFix',
        display: 'string',
        header: 'Wednesday',
        width: '150px',
        frozen: false
      },
      {
        field: 'thuFix',
        display: 'string',
        header: 'Thursday',
        width: '150px',
        frozen: false
      },
      {
        field: 'friFix',
        display: 'string',
        header: 'Friday',
        width: '150px',
        frozen: false
      },
      {
        field: 'satFix',
        display: 'string',
        header: 'Saturday',
        width: '150px',
        frozen: false
      },
      {
        field: 'sunFix',
        display: 'string',
        header: 'Sunday',
        width: '150px',
        frozen: false
      },
    ];    

    this.fixSlotCols = [      
      {
        field: 'supGroupName',
        display: 'string',
        header: 'Sup Group'
      },
      // {
      //   field: 'isVip',
      //   display: 'string',
      //   header: 'VIP'
      // }
    ];

    this.supplierGroupService.getAll().pipe(untilDestroyed(this)).subscribe((d) => {
      this.supplierGroups = d;
      this.supplierGroups.forEach((sup)=>{
        sup.supDisplay = sup.internalSupGroupId + '-' + sup.supName;
        if(sup.isVip == 'Y'){
          sup.supDisplay = sup.supDisplay + '(Fix time slot)';
        }
      });
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
    });

    
  }

  initialCapData(){
    this.capDatas = [];
    this.operationCapacities.forEach((d)=>{
      d.time = new Date(d.time);
    });

    var totalTimeSlot = 0;
    var whseData = this.warehouses.find(x=>x.warehouseCode == this.warehouseSelected);
    var min = ':00';
    var minNum = 0;
    var hour = 0;

    if(whseData.timeIncreaseStep == 30)
    {
      totalTimeSlot = 48;
    }
    else{
      totalTimeSlot = 24;
    }

    for (let index = 1; index <= totalTimeSlot; index++) {      
      if(totalTimeSlot == 48){
        if(index % 2 == 1){
          min = ':00';
          minNum = 0;
          hour = Math.ceil(index / 2);
        }
        else{
          min = ':30';
          minNum = 30;
          hour = Math.floor(index / 2);
        }
      }
      else{
        min = ':00';
        minNum = 0;
        hour = index;
      }

      if(hour == 24){
        hour = 0;
      }

      let opCap = this.operationCapacities.find(x=>x.time.getHours() == hour && x.time.getMinutes() == minNum);
      if (opCap !== null && opCap !== undefined){
        this.capDatas.push(
          {
            time: (hour).toString().padStart(2,'0') + min,
            monCap: opCap.monCap == undefined? 0 : opCap.monCap,
            monTruck: opCap.monTruck == undefined? 0 : opCap.monTruck,
            tueCap: opCap.tueCap == undefined? 0 : opCap.tueCap,
            tueTruck: opCap.tueTruck == undefined? 0 : opCap.tueTruck,
            wedCap: opCap.wedCap == undefined? 0 : opCap.wedCap,
            wedTruck: opCap.wedTruck == undefined? 0 : opCap.wedTruck,
            thuCap: opCap.thuCap == undefined? 0 : opCap.thuCap,
            thuTruck: opCap.thuTruck == undefined? 0 : opCap.thuTruck,
            friCap: opCap.friCap == undefined? 0 : opCap.friCap,
            friTruck: opCap.friTruck == undefined? 0 : opCap.friTruck,
            satCap: opCap.satCap == undefined? 0 : opCap.satCap,
            satTruck: opCap.satTruck == undefined? 0 : opCap.satTruck,
            sunCap: opCap.sunCap == undefined? 0 : opCap.sunCap,
            sunTruck: opCap.sunTruck == undefined? 0 : opCap.sunTruck,
          });
      }
      else{
        this.capDatas.push(
          {
            time: (hour).toString().padStart(2,'0') + min,
            monCap: 0,
            monTruck: 0,
            tueCap: 0,
            tueTruck: 0,
            wedCap: 0,
            wedTruck: 0,
            thuCap: 0,
            thuTruck: 0,
            friCap: 0,
            friTruck: 0,
            satCap: 0,
            satTruck: 0,
            sunCap: 0,
            sunTruck: 0
          });
      }
    }
    this.capDatas.sort((a,b) => {
      if (a.time < b.time) return -1;  // a comes before b
      if (a.time > b.time) return 1;   // b comes before a
      return 0;
    });
  }

  initialFixSlotData(){
    this.fixDatas = [];

    this.operationFixSlots.forEach((d)=>{
      d.startTime = new Date(d.startTime);
      d.endTime = new Date(d.endTime);
      let sup = this.supplierGroups.find(x=>x.internalSupGroupId == d.supGroupId);      
      if(sup.isVip == 'Y'){
        d.supGroupName = d.supGroupId.toString() + '*';       
      }
      else{
        d.supGroupName = d.supGroupId.toString();        
      }
    });

    var totalTimeSlot = 0;
    var whseData = this.warehouses.find(x=>x.warehouseCode == this.warehouseSelected);
    var min = ':00';
    var minNum = 0;
    var hour = 0;

    if(whseData.timeIncreaseStep == 30)
      {
        totalTimeSlot = 48;
      }
      else{
        totalTimeSlot = 24;
      }

    for (let index = 1; index <= totalTimeSlot; index++) {
      if(totalTimeSlot == 48){
        if(index % 2 == 1){
          min = ':00';
          minNum = 0;
          hour = Math.ceil(index / 2);
        }
        else{
          min = ':30';
          minNum = 30;
          hour = Math.floor(index / 2);
        }
      }
      else{
        min = ':00';
        minNum = 0;
        hour = index;
      }
      
      if(hour == 24){
        hour = 0;
      }
      //x=>x.time.getHours() == hour && x.time.getMinutes() == minNum

      let opFix = this.operationFixSlots.find(x=>x.startTime.getHours() == hour && x.startTime.getMinutes() == minNum);
      let opCap = this.operationCapacities.find(x=>x.time.getHours() == hour && x.time.getMinutes() == minNum);
      
      let monFix = this.operationFixSlots.filter(x=>x.startTime.getHours() == hour 
                    && x.startTime.getMinutes() == minNum
                    && x.daysOfWeek == "2").map(x=>x.supGroupName).join('\n ');
      let tueFix = this.operationFixSlots.filter(x=>x.startTime.getHours() == hour 
                    && x.startTime.getMinutes() == minNum
                    && x.daysOfWeek == "3").map(x=>x.supGroupName).join('\n ');
      let wedFix = this.operationFixSlots.filter(x=>x.startTime.getHours() == hour 
                    && x.startTime.getMinutes() == minNum
                    && x.daysOfWeek == "4").map(x=>x.supGroupName).join('\n ');
      let thuFix = this.operationFixSlots.filter(x=>x.startTime.getHours() == hour 
                    && x.startTime.getMinutes() == minNum
                    && x.daysOfWeek == "5").map(x=>x.supGroupName).join('\n ');
      let friFix = this.operationFixSlots.filter(x=>x.startTime.getHours() == hour 
                    && x.startTime.getMinutes() == minNum
                    && x.daysOfWeek == "6").map(x=>x.supGroupName).join('\n ');
      let satFix = this.operationFixSlots.filter(x=>x.startTime.getHours() == hour 
                    && x.startTime.getMinutes() == minNum
                    && x.daysOfWeek == "7").map(x=>x.supGroupName).join('\n ');
      let sunFix = this.operationFixSlots.filter(x=>x.startTime.getHours() == hour 
                    && x.startTime.getMinutes() == minNum
                    && x.daysOfWeek == "1").map(x=>x.supGroupName).join('\n ');

      if (opFix !== null && opFix !== undefined){
        this.fixDatas.push(
          {
            time: (hour).toString().padStart(2,'0') + min,            
            monCap: opCap == undefined || opCap.monCap == undefined? '-' : opCap.monCap,
            tueCap: opCap == undefined || opCap.tueCap == undefined? '-' : opCap.tueCap,
            wedCap: opCap == undefined || opCap.wedCap == undefined? '-' : opCap.wedCap,
            thuCap: opCap == undefined || opCap.thuCap == undefined? '-' : opCap.thuCap,
            friCap: opCap == undefined || opCap.friCap == undefined? '-' : opCap.friCap,
            satCap: opCap == undefined || opCap.satCap == undefined? '-' : opCap.satCap,
            sunCap: opCap == undefined || opCap.sunCap == undefined? '-' : opCap.sunCap,
            monFix: monFix.length == 0 ? '-' : monFix,
            tueFix: tueFix.length == 0 ? '-' : tueFix,
            wedFix: wedFix.length == 0 ? '-' : wedFix,
            thuFix: thuFix.length == 0 ? '-' : thuFix,
            friFix: friFix.length == 0 ? '-' : friFix,
            satFix: satFix.length == 0 ? '-' : satFix,
            sunFix: sunFix.length == 0 ? '-' : sunFix,
          });
      }
      else{
        this.fixDatas.push(
          {
            time: (hour).toString().padStart(2,'0') + min,
            monCap: opCap == undefined || opCap.monCap == undefined? '-' : opCap.monCap,
            tueCap: opCap == undefined || opCap.tueCap == undefined? '-' : opCap.tueCap,
            wedCap: opCap == undefined || opCap.wedCap == undefined? '-' : opCap.wedCap,
            thuCap: opCap == undefined || opCap.thuCap == undefined? '-' : opCap.thuCap,
            friCap: opCap == undefined || opCap.friCap == undefined? '-' : opCap.friCap,
            satCap: opCap == undefined || opCap.satCap == undefined? '-' : opCap.satCap,
            sunCap: opCap == undefined || opCap.sunCap == undefined? '-' : opCap.sunCap,
            monFix: '-',
            tueFix: '-',
            wedFix: '-',
            thuFix: '-',
            friFix: '-',
            satFix: '-',
            sunFix: '-'
          });
      }
    }

    this.fixDatas.sort((a,b) => {
      if (a.time < b.time) return -1;  // a comes before b
      if (a.time > b.time) return 1;   // b comes before a
      return 0;
    });
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

          this.operationService.getOperationCap(this.data.operationName,this.data.warehouseCode)
          .pipe(untilDestroyed(this))
          .subscribe((d)=>{
            this.operationCapacities = d;            
            this.initialCapData();

            this.operationService.getOperationFixSlot(this.data.operationName,this.data.warehouseCode)
            .pipe(untilDestroyed(this))
            .subscribe((d) => {
              this.operationFixSlots = d;
              this.operationFixSlots.forEach(data => {
                data.supGroupName = data.supGroupId + '-' + this.supplierGroups.find(x=>x.internalSupGroupId == data.supGroupId).supName;
                data.dayName = this.dayOfWeeks.find(x=>x.value == data.daysOfWeek).display;
              });
              this.initialFixSlotData();
            });
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

  onRowEditInit(data: any) {
    this.clonedCapData[data.time as string] = { ...data };
  }

  onRowEditCancel(data: any, index: number) {
    this.capDatas[index] = this.clonedCapData[data.time as string];
    delete this.clonedCapData[data.time as string];
  }

  onRowEditSave(data: any) {   
    this.isLoading = true;
    delete this.clonedCapData[data.time as string];
    // convert data to dto
    let operationCap = {} as OperationCapacity;
    let capTime = new Date();
    capTime.setHours(data.time.split(':')[0],data.time.split(':')[1],0,0);

    operationCap.warehouseCode = this.data.warehouseCode;
    operationCap.operationType = this.data.operationName;
    operationCap.time = capTime;
    operationCap.monCap = data.monCap;
    operationCap.tueCap = data.tueCap;
    operationCap.wedCap = data.wedCap;
    operationCap.thuCap = data.thuCap;
    operationCap.friCap = data.friCap;
    operationCap.satCap = data.satCap;
    operationCap.sunCap = data.sunCap;
    operationCap.monTruck = data.monTruck;
    operationCap.tueTruck = data.tueTruck;
    operationCap.wedTruck = data.wedTruck;
    operationCap.thuTruck = data.thuTruck;
    operationCap.friTruck = data.friTruck;
    operationCap.satTruck = data.satTruck;
    operationCap.sunTruck = data.sunTruck;

    this.operationService.saveOperationCap(operationCap)
    .pipe(untilDestroyed(this))
    .subscribe({ next: (d) =>{
      this.visible = false;
      this.fetchData();
      this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Capacity is updated' });    
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

  onAddFixTime(data: any,col: any){    
    var whseData = this.warehouses.find(x=>x.warehouseCode == this.warehouseSelected);
    
    //if(whseData.timeIncreaseStep == 30)
      
    let time = new Date();
    let timeEnd = new Date();
    //time.setHours(+data.time.split(':')[0],0,0,0);
    time.setHours(+data.time.split(':')[0],+data.time.split(':')[1],0,0);
    timeEnd.setHours(+data.time.split(':')[0],+data.time.split(':')[1]+whseData.timeIncreaseStep - 1,59,0);

    let numberOfDay = 0;
    switch (col.field) {
      case 'monFix':
        numberOfDay = 2;
        break;
      case 'tueFix':
        numberOfDay = 3;
        break;
      case 'wedFix':
        numberOfDay = 4;
        break;
      case 'thuFix':
        numberOfDay = 5;
        break;
      case 'friFix':
        numberOfDay = 6;
        break;
      case 'satFix':
        numberOfDay = 7;
        break;
      case 'sunFix':
        numberOfDay = 1;
        break;
    }

    this.operationService.getOperationFixSlot(this.data.operationName,this.data.warehouseCode)
    .pipe(untilDestroyed(this))
    .subscribe({next: (d) => {
      this.operationFixSlots = d;

      this.operationFixSlots.forEach((op)=>{
        op.startTime = new Date(op.startTime);
        op.endTime = new Date(op.endTime);
        op.supGroupName = op.supGroupId + '-' + this.supplierGroups.find(x=>x.internalSupGroupId == op.supGroupId).supName;
      });

      this.operationFixSlotAll = this.operationFixSlots.filter(x=>x.daysOfWeek == numberOfDay.toString() 
              && x.startTime.getHours() == time.getHours()
              && x.startTime.getMinutes() == time.getMinutes());
              
      this.fixStartTime = time;
      this.fixEndTime = timeEnd;
      this.isVip = false;
      this.supplierGroupSelected = null;
      this.dayOfWeekSelected = numberOfDay.toString();

      this.visibleFixSlot = true;
    }});
    
  }

  onDeleteFixTime(row:OperationFixSlot){   
    row.supGroup = {} as SupplierGroup; 
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
        this.operationService.deleteOperationFixSlot(row)
        .pipe(untilDestroyed(this))
        .subscribe({next:(d)=>{
          this.fetchData();
          this.isLoading = false;
          this.visibleFixSlot = false;
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

  onFixTimeSave(){
    this.isLoading = true;
    let operationFixSlot = {} as OperationFixSlot;

    operationFixSlot.warehouseCode = this.data.warehouseCode;
    operationFixSlot.operationType = this.data.operationName;
    operationFixSlot.startTime = this.fixStartTime;
    operationFixSlot.endTime = this.fixEndTime;
    operationFixSlot.daysOfWeek = this.dayOfWeekSelected;
    operationFixSlot.supGroupId = +this.supplierGroupSelected;
    operationFixSlot.supGroup = this.supplierGroups.find(x=>x.internalSupGroupId == +this.supplierGroupSelected);
    operationFixSlot.isVip = 'N'; //this.isVip ? 'Y' : 'N';
    this.operationService.saveOperationFixSlot(operationFixSlot)
    .pipe(untilDestroyed(this))
    .subscribe({ next: (d) =>{
      this.visibleFixSlot = false;
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
      this.visibleFixSlot = false;
    }});
  }

}
