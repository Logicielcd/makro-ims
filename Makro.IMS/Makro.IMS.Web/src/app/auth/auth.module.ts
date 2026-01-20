import { NgModule } from '@angular/core';
import { ThemeModule } from '@theme/theme.module';
import { AuthRoutingModule } from './auth-routing.module';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [ThemeModule, AuthRoutingModule,ReactiveFormsModule],
})
export class AuthModule {}
