import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Shunt } from '@core/models/master/shunt.model';
import { Yard } from '@core/models/master/yard.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
import { ShuntService } from '@core/services/master/shunt.service';
import { YardService } from '@core/services/master/yard.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
interface StatusOption {
  label: string;
  value: string;
}

@UntilDestroy()
@Component({
  selector: 'app-shunt-detail',
  templateUrl: './shunt-detail.component.html',
})
export class ShuntDetailComponent implements OnInit {
  dataId: number;
  data: Shunt = {} as Shunt;
  yards: Yard[] = [];
  statusOptions: StatusOption[] = [
    { label: 'Available', value: 'Available' },
    { label: 'In-Use', value: 'In-Use' },
  ];
  phonePattern = /^0\d{9}$/;

  msgs: Message[] = [];
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
    private authService: AuthService,
    private shuntService: ShuntService,
    private yardService: YardService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public route: ActivatedRoute,
    private _location: Location
  ) {
    this.authService
      .canModify(APP_MENU.master.module, APP_MENU.master.supplier)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
    this.canModify = true;
  }

  ngOnInit() {
    if (this.route.snapshot.params.id !== undefined) {
      const decodedId = atob(this.route.snapshot.params.id);
      this.dataId = +decodedId;
    }
    if (this.dataId) {
      this.fetchData();
    } else {
      this.isNew = true;
    }

    this.yardService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.yards = data.sort((a, b) => {
          const typeComparison = a.yardType.localeCompare(b.yardType);
          if (typeComparison === 0) {
            return a.yardNo.localeCompare(b.yardNo);
          }
          return typeComparison;
        });
      });
  }

  fetchData() {
    this.msgs = [];
    this.shuntService
      .getById(this.dataId)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.data = data;
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
    if (!this.validateLicensePlate(this.data.licensePlate)) {
      this.msgs.push({
        severity: 'error',
        summary: 'Error',
        detail: 'รูปแบบทะเบียนรถไม่ถูกต้อง',
      });
      return;
    }
    if (!this.dataId) {
      this.onCreate();
    } else {
      this.onUpdate();
    }
  }

  onCreate() {
    this.msgs = [];
    this.isLoading = true;
    this.shuntService
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
          this.data = {} as Shunt;
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
    this.msgs = [];
    this.shuntService
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
    this.shuntService
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

  private validateLicensePlate(licensePlate: string): boolean {
    if (!licensePlate) return false;

    const pattern =
      /^(?:\d{1}[ก-ฮ]{1,2}|\d{3}|\d{2}|[ก-ฮ]{1,2}?)[-]{1}\d{1,4}$/;
    const matches = licensePlate.match(pattern);
    return matches && matches[0] === licensePlate;
  }

  validateAndFormatPhoneNumber(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    if (value.length > 10) {
      value = value.substring(0, 10);
    }
    if (value.length > 0 && value[0] !== '0') {
      value = '0' + value.substring(0, 9);
    }
    input.value = value;
    this.data.telNo = value;
  }

  isPhoneValid(): boolean {
    return this.phonePattern.test(this.data.telNo);
  }

  onBack() {
    this._location.back();
  }
}
