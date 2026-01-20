import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserRegister } from '@core/models/auth/login-model';
import { AuthService } from 'auth/auth.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { UserService } from '@core/services/account/user.service';
import { Message, MessageService } from 'primeng/api';

@UntilDestroy()
@Component({
  selector: 'app-forgotpassword',
  templateUrl: './forgotpassword.component.html',
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
export class ForgotPasswordComponent {

  formSubmit: boolean = false;
  isLoading = false;
  confirmPassword: string = "";
  userName: string;
  supCode: string;

  error_messages = {    
    'uname': [
      { type: 'required', message: 'User login name is required.' },    
      { type: 'pattern', message: 'User login name pattern error.' }  
    ],
    'supcode': [
      { type: 'required', message: 'Supplier code is required.' },      
    ],
  }

  userRegister: UserRegister;
  msgs: Message[];
  loginForm: FormGroup;
  
  constructor(
    private userService: UserService,
    private messageService: MessageService,
    public formBuilder: FormBuilder,
    private authService: AuthService,
    public router: Router
  ) {
    this.loginForm = this.formBuilder.group({      
      uname: new FormControl('', Validators.compose([
        Validators.required,      
        Validators.pattern('([A-z0-9+-/!@#$%^&])+'),
      ])),
      supcode: new FormControl('', Validators.compose([
        Validators.required,      
      ])),            
    });
  }

  ngOnInit() {
    
  }

  register(){
    this.userRegister = {} as UserRegister;
    this.userRegister.userName = this.loginForm.get('uname').value;
    this.userRegister.supCode = this.loginForm.get('supcode').value;
    this.userRegister.email = "";
    this.userRegister.firstName = "";
    this.userRegister.lastName = "";
    this.userRegister.password = "";
   
    this.authService
    .forgotPassword(this.userRegister)
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.isLoading = false;
        this.msgs = [];
        this.msgs.push({
          severity: 'info',
          summary: 'Reset password success',
          detail: 'Check your e-mail',
        });

        this.loginForm.reset();
          // this.messageService.add({severity:'info', summary: 'Register user completed', detail: "Register user completed"});     
       //   this.router.navigate(['/']);
        
      },
      error: (error) => {
        this.msgs = [];
        error.Messages.forEach((msg: any) => {
          this.msgs.push({
            severity: 'error',
            summary: 'Error',
            detail: msg,
          });
        });
        this.isLoading = false;      
      },
    });
  }

  back(){
    this.router.navigate(['/']);
  }
}
