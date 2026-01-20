import { Location } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Role } from '@core/models/account/role.model';
import { User } from '@core/models/account/user.model';
import { SupplierGroup } from '@core/models/master/supplier.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { RoleService } from '@core/services/account/role.service';
import { UserService } from '@core/services/account/user.service';
import { SupplierGroupService } from '@core/services/master/supplier-group.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { OperationService } from '@core/services/master/operation.service';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { Message, MessageService } from 'primeng/api';
import { Warehouse } from '@core/models/master/warehouse.model';
import { Operation } from '@core/models/master/operation.model';
import { of, switchMap } from 'rxjs';
import { select } from '@syncfusion/ej2-base';

@UntilDestroy()
@Component({
  templateUrl: './user-master-detail.component.html',
})
export class UserMasterDetailComponent implements OnInit {
  msgs: Message[] = [];
  
  supplierGroups: SupplierGroup[];
  warehouses: Warehouse[];  
  operations: Operation[];

  supplierSelected: SupplierGroup[];
  warehouseSelected: Warehouse[];

  supplierGroupsAvailable: SupplierGroup[];
  warehousesAvailable: Warehouse[];

  dataId: string;
  data: User;

  userTypes: any[];

  deleteDialog: boolean = false;
  resetDialog: boolean = false;
  isLoading: boolean = false;
  canModify = true;
  issystemuser: boolean = false;

  @ViewChild('f') f: NgForm;

  translatePrefix = {
    FromApi: 'Message.FromApi',
    Validation: 'Message.Validation',
    CreateSuccess: 'Message.Notification.Create',
    EditSuccess: 'Message.Notification.Update',
    DeleteSuccess: 'Message.Notification.Delete',
    ResetPasswordSuccess: 'Message.Notification.ResetPassword',
    TestConnectionSuccess: 'Message.Notification.TestConnection',
    DeleteTitle: 'Message.Confirm.Delete.Title',
    DeleteMessage: 'Message.Confirm.Delete.Message',
    YesButton: 'Button.Yes',
    NoButton: 'Button.No',
  };

  constructor(
    public route: ActivatedRoute,
    private userService: UserService,
    private roleService: RoleService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private authService: AuthService,
    private supplierGroupService: SupplierGroupService,
    private warehouseService: WarehouseService,
    private operationService: OperationService,
    private _location: Location,
    private cdr: ChangeDetectorRef
  ) 
  {
    this.authService
      .canModify(APP_MENU.account.module, APP_MENU.account.userMaster)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
  }

  ngOnInit() {
    
    this.userTypes = [
      {id:"ADMIN", name:"System Admin"}, 
      {id:"CONTROL", name:"Control Tower"},
      {id:"DATA", name: "Data Entry"},
      {id:"OPS", name: "Operation"},
      {id:"SEC", name:"Security"},
      {id:"SUP", name:"Supplier"},
      {id:"SUPTRAN", name:"Supplier Transports"},
      {id:"REPORT",name:"Report and Dashboard"},
      {id:"SFSADMIN",name:"Siam foods service admin"},
      {id:"SUPERUSER",name: "Super user"},
      {id:"TRANSPORT", name: "Transport team"},
      {id:"WAIVE", name: 'Waive'},
      {id:"BH", name: 'Backhaul'},
      {id:"YARD", name: 'Yard'},
      {id:"DEVELOP", name: 'Developer'},
    ]

    this.dataId = this.route.snapshot.params.id;

    this.data = {} as User;
    this.fetchData();

  }


  fetchData() {
    this.msgs = [];
    this.isLoading = true;
  
    const supplierGroup$ = this.supplierGroupService.getAll().pipe(untilDestroyed(this));
    const warehouse$ = this.warehouseService.getAll().pipe(untilDestroyed(this));
  
    supplierGroup$
      .pipe(
        switchMap((supplierGroups) => {
          this.supplierGroups = supplierGroups;
          this.cdr.markForCheck();
          this.supplierSelected = [];

          this.supplierGroups.forEach((sup) => { sup.supDisplay = `${sup.supName} (${sup.internalSupGroupId})` });

          return warehouse$;
        }),
        switchMap((warehouses) => {
          this.warehouses = warehouses;
          this.isLoading = false;          
          return this.dataId ? this.userService.getUser(atob(this.dataId)).pipe(untilDestroyed(this)) : of(null);
        })
      )
      .subscribe({
        next: (user) => {
          if (user) {
            this.handleUserData(user);
          }
          this.isLoading = false;
        },
        error: (error) => this.handleError(error),
      });
  }
  
  // Handle user data
  private handleUserData(user: any) {
    this.data = user;
    this.data.isApproved = this.approvedValue(this.data.approved);

    this.supplierSelected = this.supplierGroups.filter((sup) => this.data.userSupplierGroups.some((usg) => usg.internalSupGroupId === sup.internalSupGroupId));
    this.warehouseSelected = this.warehouses.filter((whse) => this.data.userWarehouses.some((uw) => uw.warehouseCode === whse.warehouseCode));

    //if (this.data.warehouseCode) {
      this.onWhseChange();
      this.onSupplierGroupChange();
    //}
  }
  
  // Handle API errors
  private handleError(error: any) {
    this.msgs = (error.Messages || []).map((msg: any) => ({
      severity: 'error',
      summary: 'Error',
      detail: this.translateService.instant(`${this.translatePrefix.FromApi}.${msg}`),
    }));
    this.isLoading = false;
  }
  
  onWhseChange(){
    this.warehousesAvailable = this.warehouses;
    
    if(this.data.warehouseCode !== undefined && this.data.warehouseCode !== null){
      this.warehousesAvailable = this.warehouses.filter((whse) => whse.warehouseCode !== this.data.warehouseCode);
    }
    this.operationService.getByWarehouse(this.data.warehouseCode)
      .pipe(untilDestroyed(this))
      .subscribe({next: (op) => {
        this.operations = op;        
      }});
  }

  onSupplierGroupChange(){
    this.supplierGroupsAvailable = this.supplierGroups;
    if(this.data.internalSupGroupId !== undefined && this.data.internalSupGroupId !== null){
      this.supplierGroupsAvailable = this.supplierGroups.filter((sup) => sup.internalSupGroupId !== this.data.internalSupGroupId);
    }
  }

  onSubmit() {
    this.msgs = [];

    console.log('whseSelected',this.warehouseSelected);

    this.data.userSupplierGroups = [];
    this.data.userSupplierGroups.push(...this.supplierSelected.map((sup) => ({internalSupGroupId: sup.internalSupGroupId})));

    this.data.userWarehouses = [];
    this.data.userWarehouses.push(...this.warehouseSelected.map((whse) => ({warehouseCode: whse.warehouseCode})));

    console.log('data',this.data);
    
    if (!this.dataId) {
      this.onCreate();
    } else {
      this.onUpdate();
    }
  }

  onCreate() {
    if(this.data.isApproved == true){
      this.data.approved = "Y";
    }
    else{
      this.data.approved = "N";
    }

    this.isLoading = true;
    this.userService
      .createUser(this.data)
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
          this.data = {} as User;
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
        complete: () => {
          this.onBack();
        },
      });
  }
  onUpdate() {    
    if(this.data.isApproved == true){
      this.data.approved = "Y";
    }
    else{
      this.data.approved = "N";
    }

    this.userService
      .updateUser(this.data)
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
    this.deleteDialog = true;
  }

  confirmDelete() {
    this.msgs = [];
    this.deleteDialog = false;
    this.isLoading = true;

    this.userService
      .deleteUser(this.data)
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

  onResetPassword() {
    this.resetDialog = true;
  }

  confirmResetPassword() {
    this.userService
      .newPassword(this.data)
      .pipe(untilDestroyed(this))
      .subscribe((user) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Successful',
          detail: this.translateService.instant(
            `${this.translatePrefix.ResetPasswordSuccess}`
          ),
          life: 3000,
        });

        this.resetDialog = false;
        this.fetchData();
      });
  }

  approvedValue(app:string): boolean{
    if(app === 'Y'){
      return true;
    }
    else{
      return false;
    }
  }

  onBack() {
    this._location.back();
  }
}
