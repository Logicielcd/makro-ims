import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { JwtModule, JWT_OPTIONS } from '@auth0/angular-jwt';
import { environment } from '@environments/environment';
import { ThemeModule } from '@theme/theme.module';
import {
  AuthenticationGuard,
  AuthorizationGuard
} from 'auth/auth-guard.service';
import { AuthInterceptor } from 'auth/auth.interceptor';
import { AuthService } from 'auth/auth.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

export function jwtOptionFactor(authService: AuthService) {
  return {
    tokenGetter: () => {      
      return authService.getAccessToken();
    },
    // allowedDomains: [environment.apiDomain,environment.apiDomainIp],
    // disallowedRoutes: [`${environment.apiUrl}/auth/login`,`${environment.apiUrlIp}/auth/login`],
  };
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    ThemeModule,
    AppRoutingModule,
    JwtModule.forRoot({
      jwtOptionsProvider: {
        provide: JWT_OPTIONS,
        useFactory: jwtOptionFactor,
        deps: [AuthService],
      },
    }),
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    AuthenticationGuard,
    AuthorizationGuard,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
