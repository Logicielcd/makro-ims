import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Warehouse } from '@core/models/master/warehouse.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { PagingService } from '@core/services/paging.service';
import { SupplierService } from '@core/services/master/supplier.service';
import { Supplier,SupplierGroup } from '@core/models/master/supplier.model';
import { SupplierGroupService } from '@core/services/master/supplier-group.service';
import { User } from '@core/models/account/user.model';

@UntilDestroy()
@Component({
  selector: 'app-supplier-detail',
  templateUrl: './supplier-detail.component.html',
})
export class SupplierDetailComponent implements OnInit {
  msgs: Message[] = [];
  user: User;
  dataId: string;
  data: Supplier = {} as Supplier;

  supplierGroups: SupplierGroup[];
  supplierGroupSelected: number;

  isLoading: boolean = false;
  canModify = false;

  loadCompleted: boolean = false;

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
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private authService: AuthService,
    private _location: Location,
    private supplierService: SupplierService,
    private supplierGroupService: SupplierGroupService,
    private pagingService: PagingService,    
  ) {
    this.isLoading = false;
    this.user = this.authService.getUser();

    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.supplier)
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

    this.supplierGroupService.getAll()
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      this.supplierGroups = data;
      this.isLoading = false;
    },error: (error) => {
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

    if(this.route.snapshot.params.id !== undefined){
      const decodedId = atob(this.route.snapshot.params.id);
      this.dataId = decodedId;
    }
    
    if(this.dataId){
      this.fetchData();
    }
  }

  fetchData() {
    this.msgs = [];
    this.isLoading = true;
    this.supplierService
      .getById(this.dataId)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.data = data;
          this.supplierGroupSelected = this.data.internalGroupId;
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
    this.isLoading = false;
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
    this.data.userStamp = this.user.userId;
    this.data.internalGroupId = this.supplierGroupSelected;

    this.supplierService.add(this.data)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
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
      this.data = {} as Supplier;
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

  onUpdate() {
    this.isLoading = true;
    this.data.userStamp = this.user.userId;
    this.data.internalGroupId = this.supplierGroupSelected;

    this.supplierService.update(this.data)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      this.messageService.add({
        severity: 'success',
        summary: 'Successful',
        detail: this.translateService.instant(
          `${this.translatePrefix.EditSuccess}`
        ),
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

    // this.supplierService
    //   .delete(this.dataId)
    //   .pipe(untilDestroyed(this))
    //   .subscribe({
    //     next: () => {
    //       this.messageService.add({
    //         severity: 'success',
    //         summary: 'Successful',
    //         detail: this.translateService.instant(
    //           `${this.translatePrefix.DeleteSuccess}`
    //         ),
    //         life: 3000,
    //       });
    //       this.isLoading = false;
    //       this.onBack();
    //     },
    //     error: (error) => {
    //       this.msgs = [];
    //       error.Messages.forEach((msg: any) => {
    //         this.msgs.push({
    //           severity: 'error',
    //           summary: 'Error',
    //           detail: this.translateService.instant(
    //             `${this.translatePrefix.FromApi}.${msg}`
    //           ),
    //         });
    //       });
    //       this.isLoading = false;
    //     },
    //   });
      
  }

  onBack() {    
    this._location.back();    
  }

  
}
