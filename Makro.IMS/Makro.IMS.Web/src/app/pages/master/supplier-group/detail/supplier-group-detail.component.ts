import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Warehouse } from '@core/models/master/warehouse.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { OperationService } from '@core/services/master/operation.service';
import { Operation } from '@core/models/master/operation.model';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { SupplierGroup } from '@core/models/master/supplier.model';
import { SupplierGroupService } from '@core/services/master/supplier-group.service';
import { User } from '@core/models/account/user.model';
import { EstTimeService } from '@core/services/master/est-time.service';
import { EstTime } from '@core/models/master/est-time.model';

@UntilDestroy()
@Component({
  selector: 'app-supplier-group-detail',
  templateUrl: './supplier-group-detail.component.html',
})
export class SupplierGroupDetailComponent implements OnInit {
  msgs: Message[] = [];
  user: User;
  
  dataId: number;
  data: SupplierGroup = {} as SupplierGroup;

  isLoading: boolean = false;
  canModify = false;

  loadCompleted: boolean = false;

  cols: any[];
  visible: boolean;

  estTimes: EstTime[] = [];

  warehouses: Warehouse[] = [];
  warehouseSelected: string;

  warehousesVip: Warehouse[] = [];
  warehouseSelectedVip: Warehouse[] = [];

  warehouseCutoff: Warehouse[];
  warehouseSelectedCutoff: Warehouse[] = [];
  
  operations: Operation[] = [];
  operationSelected: Operation[];

  trucks: TruckMaster[] = [];
  truckSelected: string;

  estHour: number = 0;
  estMinute: number = 0;

  isAdmin: boolean;

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
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private authService: AuthService,
    private _location: Location,    
    private supplierGroupService: SupplierGroupService,    
    private warehouseService: WarehouseService,
    private operationService: OperationService,
    private truckMasterService: TruckMasterService,
    private estTimeService: EstTimeService
  ) {

    this.visible = false;
    this.isAdmin = false;
    this.cols = [
      {
        field: 'warehouseCode',
        display: 'string',        
        header: 'Warehouse',
        width: '150px',
      },  
      {
        field: 'operationType',
        display: 'string',        
        header: 'Operation Type',
        width: '150px',
      },  
      {
        field: 'truckType',
        display: 'string',        
        header: 'Truck type',
        width: '150px',
      },      
      {
        field: 'hourEst',
        display: 'string',
        header: 'Est. Hours',
      },            
      {
        field: 'minEst',
        display: 'string',
        header: 'Est. Minutes',
      },            
    ];

    this.user = this.authService.getUser();

    this.isAdmin = this.user.userType === 'ADMIN' || this.user.userType === 'DEVELOP' ? true : false;

    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.supplierGroup)
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

    if(this.route.snapshot.params.id !== undefined){
      const decodedId = atob(this.route.snapshot.params.id);
      this.dataId = +decodedId;
    }

    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.warehouses = data;
        this.isLoading = false;   
        this.warehousesVip = data;
        this.warehouseCutoff = data;
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

    this.truckMasterService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.trucks = data;           
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

    if(this.dataId){
      this.fetchData();
    }
    this.isLoading = false;
  }

  fetchData() {
    this.warehouseSelectedVip = [];
    this.warehouseSelectedCutoff = [];
    this.msgs = [];
    this.isLoading = true;
    this.supplierGroupService
      .getById(this.dataId)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {          
          if(data.remark == "PL"){
            data.isPallet = true;
          }
          else{
            data.isPallet = false;
          }
          if(data.isUpCreateBook == "Y"){
            data.isCreateBook = true;
          }
          else{
            data.isCreateBook = false;
          }      
          if(data.isUpPreCheckin == "Y"){
            data.isPreCheckIn = true;
          }
          else{
            data.isPreCheckIn = false;
          }          
          if(data.postpond == "Y"){
            data.isPostPond = true;
          }
          else{
            data.isPostPond = false;
          }          
          if(data.isVip == "Y"){
            data.isVipBoolean = true;
            if(data.warehouses != null)
              data.warehouses.split('|').forEach(x=>this.warehouseSelectedVip.push(this.warehouses.find(y=>y.warehouseCode == x)));
          }
          else{
            data.isVipBoolean = false;
          }
          if(data.warehouseCutoff != null){
            data.warehouseCutoff.split('|').forEach(x=>this.warehouseSelectedCutoff.push(this.warehouses.find(y=>y.warehouseCode == x)));
          }

          this.data = data;
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

      this.estTimeService.getBySupGroupId(this.dataId)
      .pipe(untilDestroyed(this))
      .subscribe({next: (data) => {
        this.estTimes = data;
        
        this.estTimes.forEach(est => {
          est.truckType = this.trucks.find(x=>x.internalTruckId == est.internalTruckId).truckCode;
          //console.log(this.trucks.find(x=>x.internalTruckId == est.internalTruckId).truckCode);
        });

        this.estTimes.sort((a,b)=>a.warehouseCode.localeCompare(b.warehouseCode));

        this.isLoading = false;
      }
      ,error: (error) =>{
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

      this.isLoading = false;
  }

  onSubmit() {

    let whseVip: string[] = [];
    if(this.data.isVipBoolean){
        this.warehouseSelectedVip.forEach(t => {
        whseVip.push(t.warehouseCode);
      });
    }

    let whseCutoff: string[] = [];
    this.warehouseSelectedCutoff.forEach(t => {
      whseCutoff.push(t.warehouseCode);
    });

    this.msgs = [];
    if (!this.dataId) {
      this.data.isConfirmGatePass = this.data.isConfirmGatePass == undefined ? false : this.data.isConfirmGatePass;
      this.data.isFixTime = this.data.isFixTime == undefined ? false : this.data.isFixTime;
      this.data.isUpCreateBook = this.data.isCreateBook == undefined ? 'N' : this.data.isUpCreateBook;
      this.data.isUpPreCheckin = this.data.isPreCheckIn == undefined ? 'N' : this.data.isUpCreateBook;
      this.data.isVip = this.data.isVipBoolean == undefined ? 'N' : this.data.isVip;
      this.data.warehouses = this.data.isVipBoolean ? whseVip.join('|') : '';
      this.data.warehouseCutoff = this.warehouseSelectedCutoff.length > 0 ? whseCutoff.join('|') : '';
      this.onCreate();
    } else {
      this.data.warehouses = this.data.isVipBoolean ? whseVip.join('|') : '';
      this.data.warehouseCutoff = this.warehouseSelectedCutoff.length > 0 ? whseCutoff.join('|') : '';
      this.onUpdate();
    }
  }

  onLoadingTypeChange(){
    if(this.data.isPallet == true){
      this.data.remark = "PL";
    }
    else{
      this.data.remark = "CS";
    }
  }
  
  onPostpondChange(){
    if(this.data.isPostPond == true){
      this.data.postpond = "Y";
    }
    else{
      this.data.postpond = "N";
    }
  }

  onIsUpBookingChange(){
    if(this.data.isCreateBook == true){
      this.data.isUpCreateBook = "Y";
    }
    else{
      this.data.isUpCreateBook = "N";
    }
  }

  onIsUpPreCheckInChange(){
    if(this.data.isPreCheckIn == true){
      this.data.isUpPreCheckin = "Y";
    }
    else{
      this.data.isUpPreCheckin = "N";
    }
  }

  onIsVipChange(){
    if(this.data.isVipBoolean == true){
      this.data.isVip = "Y";
    }
    else{
      this.data.isVip = "N";
    }
  }

  onCreate() {    
    
    if (this.data.isVipBoolean == undefined){
      this.data.isVip = 'N';
    }

    this.isLoading = true;
    this.data.userStamp = this.user.userId;
    this.supplierGroupService.add(this.data)
    .pipe(untilDestroyed(this))
    .subscribe({next:(d) => {
      this.messageService.add({
        severity: 'success',
        summary: 'Successful',
        detail: this.translateService.instant(
          `${this.translatePrefix.CreateSuccess}`
        ),
        life: 3000,
      });
      this.isLoading = false;
      this.f.resetForm();
      this.data = {} as SupplierGroup;
    },error:(error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
        this.msgs.push({
          severity: 'error',
          summary: 'Error',
          detail: this.translateService.instant(
            `${this.translatePrefix.FromApi}.${msg}`
          ),
        })
      });
      this.isLoading = false;
    }});
  }

  onUpdate() {    
    this.isLoading = true;
    this.data.userStamp = this.user.userId;
    this.supplierGroupService.updateSupGroup(this.data)
    .pipe(untilDestroyed(this))
    .subscribe({next:(d) => {
      this.messageService.add({
        severity: 'success',
        summary: 'Successful',
        detail: this.translateService.instant(
          `${this.translatePrefix.EditSuccess}`
        ),
        life: 3000,
      });
      this.isLoading = false;
      this.fetchData();
      this.f.form.markAsPristine();      
    },error:(error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
        this.msgs.push({
          severity: 'error',
          summary: 'Error',
          detail: this.translateService.instant(
            `${this.translatePrefix.FromApi}.${msg}`
          ),
        })
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

    this.supplierGroupService
      .deleteSupGroup(this.data)
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

  onEstAdd(){
    this.visible = true;
  }

  onEstDel(delData:any){
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
        this.estConfirmDelete(delData);
      },
    });
  }

  estConfirmDelete(delData : any) {
    this.msgs = [];
    this.isLoading = true;

    this.estTimeService
      .delete(delData.internalEstId)
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
          this.fetchData();
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


  onEstSave(){
    let estTime = {} as EstTime;

    let operationString = [];
    // convert truck object to string
    this.operationSelected.forEach(t => {
      operationString.push(t.operationName);
    });

    estTime.hourEst = this.estHour;
    estTime.minEst = this.estMinute;
    estTime.internalSupGroupId = this.data.internalSupGroupId;
    estTime.internalTruckId = this.trucks.find(x=>x.truckCode == this.truckSelected).internalTruckId;
    estTime.warehouseCode = this.warehouseSelected;
    estTime.operationType =  operationString.join('|');

    this.estTimeService.add(estTime)
    .pipe(untilDestroyed(this))
    .subscribe({next:(d) => {
      this.messageService.add({
        severity: 'success',
        summary: 'Successful',
        detail: this.translateService.instant(
          `${this.translatePrefix.EditSuccess}`
        ),
        life: 3000,
      });
      this.isLoading = false;
      this.fetchData();
      this.f.form.markAsPristine();      
    },error:(error) => {
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
        this.msgs.push({
          severity: 'error',
          summary: 'Error',
          detail: this.translateService.instant(
            `${this.translatePrefix.FromApi}.${msg}`
          ),
        })
      });
      this.isLoading = false;
    }});
  }

  onWarehouseChange(){
    // this.warehouseSelected = event.value;
    this.operationService.getByWarehouse(this.warehouseSelected)
          .pipe(untilDestroyed(this))
          .subscribe((d)=>{             
            this.operations = d;             
          });
  }

}
