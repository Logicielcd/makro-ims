import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot
} from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class AuthenticationGuard implements CanActivate {
  constructor(
    private jwtHelper: JwtHelperService,
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate() {
    
    const localStorageTokens = this.authService.getToken();
    
    if (!localStorageTokens) {
      this.router.navigate(['/auth/login']);
    }

    var isTokenExpired = this.jwtHelper.isTokenExpired(
      localStorageTokens.accessToken
    );


    if (isTokenExpired) {
      let curDate = new Date();
      let diffTime = curDate.getTime() - this.jwtHelper.getTokenExpirationDate(localStorageTokens.accessToken).getTime();
      const diffHour = diffTime / 3600000;

      if(diffHour >= 3){
        this.router.navigate(['/auth/login']);
      }
      else{
        this.authService.refreshToken(localStorageTokens);
      }      
      
    }

    return true;
  }
}

@Injectable()
export class AuthorizationGuard implements CanActivate {
  constructor(private authService: AuthService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (route.data.modifyOnly) {
      return this.authService.canModify(route.data.module, route.data.page);
    }

    return this.authService.canView(route.data.module, route.data.page);
  }
}
