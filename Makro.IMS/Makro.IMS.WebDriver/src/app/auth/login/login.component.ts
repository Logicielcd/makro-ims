import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginModel } from '@core/models/auth/login-model';
import { LayoutService } from '@core/services/layout/layout.service';
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

  @ViewChild('f') f: NgForm;

  constructor(
    public layoutService: LayoutService,
    private authService: AuthService,
    public router: Router
  ) {}

  userLogin() {
    this.msgs = [];
    this.isLoading = true;

    this.login.username = btoa(this.login.username);
    this.login.password = btoa(this.generateRandomString(10));

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

  
  generateRandomString(length: number): string {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
      const randomIndex = Math.floor(Math.random() * charactersLength);
      result += characters.charAt(randomIndex);
    }
    return result;
  }
  
}
