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

@UntilDestroy()
@Component({
  selector: 'app-door-detail',
  templateUrl: './door-detail.component.html',
})
export class DoorDetailComponent implements OnInit {
  msgs: Message[] = [];

  dataId: number;
  data: Door = {} as Door;

  // warehouse variable
  warehouses: Warehouse[];
  warehouseSelected: string;

  isLoading: boolean = false;
  canModify = false;

  loadCompleted: boolean = false;

  operationString: string[] = [];
  operations: Operation[] = [];
  operationSelected: Operation[];

  truckTypes: string[] = [];
  trucks: TruckMaster[] = [];
  truckSelected!: TruckMaster[];

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
    private doorService: DoorService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private authService: AuthService,
    private _location: Location,
    private truckMasterService: TruckMasterService,
    private operationService: OperationService,
    private pagingService: PagingService,
  ) {

    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.door)
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

    this.truckMasterService.getAll().pipe(untilDestroyed(this))
    .subscribe((data)=>{
      this.trucks = data;
    });

    this.warehouseService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.warehouses = data;

        if(this.route.snapshot.params.id !== undefined){
          const decodedId = atob(this.route.snapshot.params.id);
          this.dataId = +decodedId;
        }
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

  fetchData() {
    this.msgs = [];
    this.isLoading = true;
    this.doorService
      .getById(this.dataId)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.data = data;
          this.warehouseSelected = this.data.warehouseCode;
          
          if(this.data.truckType != null){
            this.truckTypes = this.data.truckType.split('|');
          }
          if(this.data.doorArea != null){
            this.operationString = this.data.doorArea.split('|');
          }

          this.operationSelected = [];
          this.operationService.getByWarehouse(this.data.warehouseCode)
          .pipe(untilDestroyed(this))
          .subscribe((d)=>{ 
            this.operations = d;       
            this.operationString.forEach(t => {
              this.operationSelected.push(this.operations.find(x=>x.operationName === t))
            });
            
            this.truckSelected = [];
            // convert trucktypes to truck object
            this.truckTypes.forEach(t => {
              this.truckSelected.push(this.trucks.find(x=>x.truckCode === t))
            });

            this.loadCompleted = true;

          });
          
          
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
    this.operationService.getByWarehouse(this.warehouseSelected)
          .pipe(untilDestroyed(this))
          .subscribe((d)=>{             
            this.operations = d; 
            this.operationString.forEach(t => {
              this.operationSelected.push(this.operations.find(x=>x.operationName === t))
            });
          });
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
    this.data.truckType = this.truckTypes.join('|');
    this.data.userStamp = '';
    this.data.doorArea = this.operationString.join('|');
    this.doorService
      .add(this.data)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
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
          this.data = {} as Door;
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

  onUpdate() {
    this.isLoading = true;
    this.data.truckType = this.truckTypes.join('|');
    this.data.doorArea = this.operationString.join('|');
    this.doorService
      .update(this.data)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Successful',
            detail: this.translateService.instant(
              `${this.translatePrefix.EditSuccess}`
            ),
            life: 3000,
          });
          this.fetchData();
          this.f.form.markAsPristine();
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

    this.doorService
      .delete(this.dataId)
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
    // let paths = this.router.url.split('/');    
    // let path = '';
    // for (let index = 0; index < paths.length-1; index++) {
    //   path += paths[index] + '/'
    // }
    // this.pagingService.isBack = true;
    // this.router.navigate([path], {
    //   queryParams: {
    //     page: this.pagingService.curPage,
    //     pageSize: this.pagingService.pageSize,
    //   },
    // });
  }

  onValidateTruckType(tag: any): boolean{    
    this.truckTypes = [];
    // convert truck object to string
    this.truckSelected.forEach(t => {
      this.truckTypes.push(t.truckCode);
    });

    // if(tag.value === '4 W' || tag.value === '6 W' || tag.value === '10 W' || tag.value === '18 W'){
      
    //   if(this.truckTypes.filter(data => data === tag.value).length > 1){
    //     this.truckTypes = [...new Set(this.truckTypes)];
    //   }
    //   return true;
    // }
    // else{      
    //   this.messageService.add({
    //     severity: 'error',
    //     summary: 'ประเภทรถไม่ถูกต้อง',
    //     detail: '',
    //     life: 3000,
    //   });

    //   this.truckTypes.pop();      
    //   return false;
    // }    
    return true;
  }

  onDoorAreaChange(tag: any): boolean{    
    this.operationString = [];
    // convert truck object to string
    this.operationSelected.forEach(t => {
      this.operationString.push(t.operationName);
    });
    
    return true;
  }

  
}
