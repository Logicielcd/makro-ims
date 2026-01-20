import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Yard } from '@core/models/master/yard.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { LanguageService } from '@core/services/language.service';
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
  selector: 'app-yard-detail',
  templateUrl: './yard-detail.component.html',
})
export class YardDetailComponent implements OnInit {
  dataId: number;
  data: Yard = {} as Yard;

  statusOptions: StatusOption[] = [
    { label: 'Available', value: 'Available' },
    { label: 'Not Available', value: 'Not Available' },
  ];

  yard_types: StatusOption[] = [
    { label: 'Yard', value: 'Yard' },
    { label: 'Dock', value: 'Dock' },
    { label: 'Maintenance', value: 'Maintenance' },
    { label: 'Washing', value: 'Washing' },
  ];

  trailer_types: StatusOption[] = [
    { label: 'Container', value: 'Container' },
    { label: 'Side Curtain', value: 'SideCurtain' },
    { label: 'Other', value: 'Other' },
  ];
  trailer_type_selected!: StatusOption[];

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
  }

  fetchData() {
    this.msgs = [];
    this.yardService
      .getById(this.dataId)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.data = data;
          // เพิ่มการเช็คค่า
          if (
            data.trailerType &&
            data.trailerType.trim() !== '' &&
            data.trailerType !== ' | '
          ) {
            this.trailerTypeString = data.trailerType;
          } else {
            this.trailer_type_selected = []; // ถ้าไม่มีข้อมูลให้ reset เป็น array ว่าง
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

  onSubmit() {
    this.msgs = [];
    if (!this.dataId) {
      this.onCreate();
    } else {
      this.onUpdate();
    }
  }

  onCreate() {
    this.msgs = [];
    this.isLoading = true;
    const dataToSubmit = {
      ...this.data,
      trailerType: this.trailerTypeString,
    };
    this.yardService
      .add(dataToSubmit)
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
          this.data = {} as Yard;
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
    const dataToSubmit = {
      ...this.data,
      trailerType: this.trailerTypeString,
    };
    this.yardService
      .update(dataToSubmit)
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
    this.yardService
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

  prepareDataForBackend() {
    const dataToSend = {
      ...this.data,
      trailerType: this.trailerTypeString,
    };
    return dataToSend;
  }
  get trailerTypeString(): string {
    if (
      !this.trailer_type_selected ||
      this.trailer_type_selected.length === 0
    ) {
      return '';
    }
    const values = this.trailer_type_selected.map((type) => type.value);
    return values.join(' | ');
  }

  set trailerTypeString(value: string) {
    if (!value || value.trim() === '' || value === ' | ') {
      this.trailer_type_selected = [];
      return;
    }
    const selectedValues = value
      .split(' | ')
      .filter((v) => v && v.trim() !== '');
    this.trailer_type_selected = this.trailer_types.filter((type) =>
      selectedValues.includes(type.value)
    );
  }

  onBack() {
    this._location.back();
  }
}
