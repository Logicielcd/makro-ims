import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Warehouse } from '@core/models/master/warehouse.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { WarehouseCapacityService } from '@core/services/master/warehouse-capacity.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { Observable } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'app-warehouse-detail',
  templateUrl: './warehouse-detail.component.html',
})
export class WarehouseDetailComponent implements OnInit {
  msgs: Message[] = [];

  dataId: string;
  data: Warehouse = {} as Warehouse;

  isLoading: boolean = false;
  canModify = true;
  isNew: boolean = false;

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
    private _location: Location
  ) {

    // this.authService
    //   .canModify(APP_MENU.master.module, APP_MENU.master.warehousecapacity)
    //   .pipe(untilDestroyed(this))
    //   .subscribe((granted) => (this.canModify = granted));

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
      this.dataId = decodedId;    
    }
    if (this.dataId) {
      this.isNew = false;
      this.fetchData();
    }
    else{
      this.isNew = true;
    }
  }

  fetchData() {
    this.msgs = [];
    this.isLoading = true;
    this.warehouseService
      .getById(this.dataId)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          
          if(data.fixDoor == 'Y')
          {
            data.isFixDoor = true;
          }
          else{
            data.isFixDoor = false;
          }

          if(data.capUom == 'CS'){
            data.isCapCS = true;
          }
          else{
            data.isCapCS = false;
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
    this.warehouseService
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
          this.data = {} as Warehouse;
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

    this.data.fixDoor = this.data.isFixDoor == true ? 'Y' : 'N';

    this.warehouseService
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

    this.warehouseService
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

  onFixDoorChange(){
    if(this.data.isFixDoor == true){
      this.data.fixDoor = 'Y';
    }
    else{
      this.data.fixDoor = 'N';
    }
  }

  onCapUomChange(){
    if(this.data.isCapCS == true){
      this.data.capUom = 'CS';
    }
    else{
      this.data.capUom = 'KG';
    }
  }

  onBack() {
    this._location.back();
  }

}
