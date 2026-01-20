import { NgModule } from '@angular/core';
import { ThemeModule } from '@theme/theme.module';
import { LoginRoutingModule } from './login-routing.module';
import { LoginComponent } from './login.component';
import { PasswordModule } from "primeng/password";
import { DividerModule } from "primeng/divider";
import { RegisterComponent } from './register.component';

@NgModule({
  imports: [ThemeModule, LoginRoutingModule,PasswordModule,DividerModule],
  declarations: [LoginComponent,RegisterComponent],
})
export class LoginModule {}
