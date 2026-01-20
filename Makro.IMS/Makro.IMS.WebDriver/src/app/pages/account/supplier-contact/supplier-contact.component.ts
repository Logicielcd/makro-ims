import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { UserService } from '@core/services/account/user.service';
import { MenuItem, Message, MessageService,ConfirmationService } from 'primeng/api';

/* service */
import { SupplierService } from '@core/services/master/supplier.service';

/* model */
import { User } from '@core/models/account/user.model';
import { UserRegister } from '@core/models/auth/login-model';
import { Supplier } from '@core/models/master/supplier.model';

@UntilDestroy()
@Component({
  templateUrl: './supplier-contact.component.html',
  styles: [
    `
      :host ::ng-deep .p-password input {
        width: 100%;
        padding: 1rem;
      }

      :host ::ng-deep .pi-eye {
        transform: scale(1.6);
        margin-right: 1rem;
        color: var(--primary-color) !important;
      }

      :host ::ng-deep .pi-eye-slash {
        transform: scale(1.6);
        margin-right: 1rem;
        color: var(--primary-color) !important;
      }

      .error-message {
        color: red;
      }
    `,
  ],
})
export class SupplierContactComponent implements OnInit {

  title = 'Reset Password';

  user: User;
  data: User;
  msgs: Message[] = [];

  userRegister: UserRegister;

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  confirmPassword: string = "";

  suppliers: Supplier[] = [];
  supplier: Supplier;
  updateSupplier: Supplier;
  
  @ViewChild('f') f: NgForm;

  error_messages = {
    'password': [
      { type: 'required', message: 'password is required.' },
      { type: 'minlength', message: 'password length.' },
      { type: 'maxlength', message: 'password length.' },
      { type: 'pattern', message: 'password pattern error.' }
    ],
    'confirmpassword': [
      { type: 'required', message: 'confirm password is required.' },
      { type: 'notmatch', message: 'confirm password not match'}
      // { type: 'minlength', message: 'password length.' },
      // { type: 'maxlength', message: 'password length.' }
    ],   
  }

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(    
    public formBuilder: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,   
    private supplierService: SupplierService, 
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

      // this.loginForm = this.formBuilder.group({
      //   password: new FormControl('', Validators.compose([
      //     Validators.required,
      //     Validators.minLength(8),
      //     Validators.maxLength(16),
      //     Validators.pattern('^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,}$'),        
      //   ])),
      //   confirmpassword: new FormControl('', Validators.compose([
      //     Validators.required,        
      //   ])),
      // }, { 
      //   validators: this.password.bind(this)
      // });
  }

  ngOnInit() {
    this.user = this.authService.getUser();
    this.data = this.user;
    
    this.supplierService
    .getBySupGroup(this.user.internalSupGroupId)
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.isLoading = false;
        this.suppliers = data;        
        this.supplier = this.suppliers[0];
      },
      error: (error) => {
        this.msgs = [];
        error.Messages.forEach((msg: any) => {
          this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000}); 
        });
        this.isLoading = false;      
      },
    });

  }

  // password(formGroup: FormGroup) {
  //   const { value: password } = formGroup.get('password');
  //   const { value: confirmPassword } = formGroup.get('confirmpassword');

  //   if (password === confirmPassword) {
  //     this.confirmPassword = "";
  //   }
  //   else{
  //     this.confirmPassword = "Confirm password not match";
  //   }

  //   return password === confirmPassword ? null : { error : "Confirm password not match"};
  //   // {       passwordNotMatch: true
  //   // };
  // }

  updateContact()
  {
    this.updateSupplier = {} as Supplier;
    this.updateSupplier.supCode = this.supplier.supCode;
    this.updateSupplier.supName = this.supplier.supName;
    this.updateSupplier.internalGroupId = this.supplier.internalGroupId;
    this.updateSupplier.internalSupGroupId = this.supplier.internalSupGroupId;
    this.updateSupplier.internalSupId = this.supplier.internalSupId;
    this.updateSupplier.contactName = this.supplier.contactName;
    this.updateSupplier.contactEMail = this.supplier.contactEMail;
    this.updateSupplier.phoneNumber = this.supplier.phoneNumber;
    this.updateSupplier.mobileNumber = this.supplier.mobileNumber;

    this.supplierService
    .updateSupContact(this.updateSupplier)
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.isLoading = false;
        this.messageService.add({severity:'info', summary: 'Update supplier contact success', detail: "Update supplier contact success",life:3000});  
      },
      error: (error) => {
        this.msgs = [];
        error.Messages.forEach((msg: any) => {
          this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000}); 
        });
        this.isLoading = false;      
      },
    });

  }
}