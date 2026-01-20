import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginModel } from '@core/models/auth/login-model';
import { LayoutService } from '@core/services/layout/layout.service';
import { environment } from '@environments/environment';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { AuthService } from 'auth/auth.service';
import { Message } from 'primeng/api';

@UntilDestroy()
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
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
    `,
  ],
})
export class LoginComponent {
  login: LoginModel = {} as LoginModel;
  formSubmit: boolean = false;
  isLoading = false;

  msgs: Message[];

  clientOffset: any;
  clientTimeZone: string;
  clientOffsetText: string;

  titlePage: string;

  @ViewChild('f') f: NgForm;

  constructor(
    public layoutService: LayoutService,
    private authService: AuthService,
    public router: Router
  ) {

    let sign = '-';
    this.clientTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

    this.clientOffset = new Date().getTimezoneOffset();

    if(this.clientOffset < 0){
      this.clientOffset = this.clientOffset * -1;
      sign = '+';
    }        

    this.clientOffsetText = '(UTC' + sign + Math.floor(this.clientOffset/60).toString() 
    + ((this.clientOffset%60) > 0 ? ':' + (this.clientOffset%60).toString() : '') + ')' ;

    this.titlePage = environment.titlePage;
  }

  userLogin() {
    this.msgs = [];
    this.isLoading = true;
    
    this.authService
      .userLogin(this.login)
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (data) => {
          this.isLoading = false;
          if (data) {
            this.router.navigate(['/']);
          }
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
          this.f.resetForm();
          this.login = {} as LoginModel;
        },
      });
  }

  validateTimeZone():boolean{
    this.clientOffset = new Date().getTimezoneOffset(); // ค่า offset ของ client ในหน่วยนาที
    const requiredOffset = -420; // Offset -420 หมายถึง UTC+7
  
    // ตรวจสอบว่า timezone ของ client ตรงกับที่ต้องการหรือไม่
    if (this.clientOffset !== requiredOffset) {
      // ถ้า timezone ไม่ตรงกัน ให้ redirect ไปหน้า login
      //window.location.href = "/login"; // ระบุ URL ของหน้า login
      return false;
    }
    return true;
  }
  
}
