import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseOperationCapacity } from '@core/models/master/warehouse-operation-capacity.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { WarehouseOperationCapacityService } from '@core/services/master/warehouse-operation-capacity.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { OperationService } from '@core/services/master/operation.service';

@UntilDestroy()
@Component({
  selector: 'app-warehouse-operation-capacity-detail',
  templateUrl: './warehouse-operation-capacity-detail.component.html',
})
export class WarehouseOperationCapacityDetailComponent implements OnInit {
  msgs: Message[] = [];

  dataId: number;
  data: WarehouseOperationCapacity = {} as WarehouseOperationCapacity;

  isLoading: boolean = false;
  canModify = false;

  operationTypes: any[] = [];
  warehouse$: Observable<Warehouse[]>;

  selectedWarehouse: string;


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
    private warehouseOperationCapacityService: WarehouseOperationCapacityService,
    private warehouseService: WarehouseService,
    private operationService: OperationService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private authService: AuthService,
    private _location: Location
  ) {

    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.warehouseOperationCapacity)
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

    if(this.route.snapshot.params.id !== undefined){
      const decodedId = atob(this.route.snapshot.params.id);
      this.dataId = +decodedId;
    }
    if (this.dataId) {
      this.fetchData();
    }
   
    this.warehouse$ = this.warehouseService.getAll();

  }

  fetchData() {
    this.msgs = [];
    this.isLoading = true;
    this.warehouseOperationCapacityService
      .getById(this.dataId)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
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
  }

  onWarehouseChange(){
    this.operationService.getByWarehouse(this.selectedWarehouse)
    .subscribe({
      next: (data)=>{
        this.operationTypes = data;
      }
    })
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
    this.warehouseOperationCapacityService
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
          this.data = {} as WarehouseOperationCapacity;
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
    this.warehouseOperationCapacityService
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

    this.warehouseOperationCapacityService
      .delete(this.data.id)
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

}
