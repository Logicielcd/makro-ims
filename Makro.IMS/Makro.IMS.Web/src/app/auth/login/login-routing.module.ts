import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './login.component';
import { RegisterComponent } from './register.component';
import { ForgotPasswordComponent } from './forgotpassword.component';
import { DuplicateLoginComponent } from './duplicatelogin.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: LoginComponent,
      },
      {
        path: 'register',
        component: RegisterComponent,
      },
      {
        path: 'forgotpassword',
        component: ForgotPasswordComponent
      },
      {
        path: 'duplicate',
        component: DuplicateLoginComponent
      }
    ]),
  ],
  exports: [RouterModule],
})
export class LoginRoutingModule {}
