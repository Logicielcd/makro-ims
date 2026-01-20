import { Component, OnInit } from '@angular/core';
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

@UntilDestroy()
@Component({
  templateUrl: './reset-password.component.html',
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
export class ResetPasswordComponent implements OnInit {

  title = 'Reset Password';

  user: User;
  data: User;
  msgs: Message[] = [];

  userRegister: UserRegister;

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  confirmPassword: string = "";

  formSubmit: boolean = false;
  loginForm: FormGroup;
  
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
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

      this.loginForm = this.formBuilder.group({
        password: new FormControl('', Validators.compose([
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(16),
          Validators.pattern('^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,}$'),        
        ])),
        confirmpassword: new FormControl('', Validators.compose([
          Validators.required,        
        ])),
      }, { 
        validators: this.password.bind(this)
      });
  }

  ngOnInit() {
    this.user = this.authService.getUser();
    this.data = this.user;
    // console.log(this.user);
  }

  password(formGroup: FormGroup) {
    const { value: password } = formGroup.get('password');
    const { value: confirmPassword } = formGroup.get('confirmpassword');

    if (password === confirmPassword) {
      this.confirmPassword = "";
    }
    else{
      this.confirmPassword = "Confirm password not match";
    }

    return password === confirmPassword ? null : { error : "Confirm password not match"};
    // {       passwordNotMatch: true
    // };
  }

  resetpassword(){
    this.userRegister = {} as UserRegister;
    this.userRegister.firstName = this.user.name;
    this.userRegister.lastName = '';
    this.userRegister.userName = this.user.userId;
    this.userRegister.password = this.loginForm.get('password').value;
    this.userRegister.email = "-";
    this.userRegister.supCode = "-";

    this.userService
    .resetPassword(this.userRegister)
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.isLoading = false;
        this.messageService.add({severity:'info', summary: 'Reset password success', detail: "Please sign out and sign in again",life:3000});  
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