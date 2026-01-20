import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserRegister } from '@core/models/auth/login-model';
import { AuthService } from 'auth/auth.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { UserService } from '@core/services/account/user.service';
import { Message, MessageService } from 'primeng/api';
import { ValidatePasswordConfirmation } from 'auth/ValidatePasswordConfirmation';

@UntilDestroy()
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
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
export class RegisterComponent {

  formSubmit: boolean = false;
  isLoading = false;
  confirmPassword: string = "";

  error_messages = {
    'fname': [
      { type: 'required', message: 'First Name is required.' },
    ],

    'lname': [
      { type: 'required', message: 'Last Name is required.' }
    ],
    'uname': [
      { type: 'required', message: 'User login name is required.' },    
      { type: 'pattern', message: 'User login name pattern error.' }  
    ],
    'supcode': [
      { type: 'required', message: 'Supplier code is required.' },      
    ],
    'password': [
      { type: 'required', message: 'password is required.' },
      { type: 'minlength', message: 'password length.' },
      { type: 'maxlength', message: 'password length.' },
      { type: 'pattern', message: 'password pattern error.' }
    ],
    'confirmpassword': [
      { type: 'required', message: 'confirm password is required.' },
      { type: 'notmatch', message: 'confirm password not match'}      
    ],
    'email': [
      { type: 'required', message: 'email is required.' },      
      { type: 'pattern', message: 'email pattern error.' }
    ],
  }

  userRegister: UserRegister;
  msgs: Message[];
  loginForm: FormGroup;
  

  constructor(
    public formBuilder: FormBuilder,
    private userService: UserService,
    private messageService: MessageService,
    private authService: AuthService,
    public router: Router
  ) {
    this.loginForm = this.formBuilder.group({
      fname: new FormControl('', Validators.compose([
        Validators.required
      ])),
      lname: new FormControl('', Validators.compose([
        Validators.required
      ])),
      uname: new FormControl('', Validators.compose([
        Validators.required,      
        Validators.pattern('([A-z0-9+-/!@#$%^&])+'),
      ])),
      supcode: new FormControl('', Validators.compose([
        Validators.required,      
      ])),
      password: new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(16),
        Validators.pattern('^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,}$'),        
      ])),
      email: new FormControl('', Validators.compose([
        Validators.required,        
        Validators.pattern('^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9]+\.{2,}$'),
      ])),
      confirmpassword: new FormControl('', Validators.compose([
        Validators.required,        
      ])),
    }, { 
      validators: this.password.bind(this)
    });
  }

  ngOnInit() {
    
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

  register(){
    this.userRegister = {} as UserRegister;
    this.userRegister.firstName = this.loginForm.get('fname').value;
    this.userRegister.lastName = this.loginForm.get('lname').value;
    this.userRegister.userName = this.loginForm.get('uname').value;
    this.userRegister.supCode = this.loginForm.get('supcode').value;
    this.userRegister.password = this.loginForm.get('password').value;
    this.userRegister.email = this.loginForm.get('email').value;

    this.authService
    .userRegister(this.userRegister)
    .pipe(untilDestroyed(this))
    .subscribe({
      next: (data) => {
        this.isLoading = false;
        this.msgs = [];
        this.msgs.push({
          severity: 'info',
          summary: 'Register user success',
          detail: 'On approved process',
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
