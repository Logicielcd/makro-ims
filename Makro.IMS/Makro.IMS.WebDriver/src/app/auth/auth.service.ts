import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from '@core/models/account/user.model';
import { UserRegister } from '@core/models/auth/login-model';
import { LoginModel } from '@core/models/auth/login-model';
import { TokenModel } from '@core/models/auth/token-model';
import { PageAction } from '@core/models/menu/auth-menu.model';
import { environment } from '@environments/environment';
import {
  BehaviorSubject,
  catchError,
  map,
  Observable,
  of,
  throwError
} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private httpClient: HttpClient) {}
  userProfile = new BehaviorSubject<User>(null);
  currentUser$ = this.userProfile.asObservable();

  jwtService: JwtHelperService = new JwtHelperService();

  setToken(token: TokenModel) {
    var temp = {
      accessToken: token.accessToken,
      refreshToken: token.refreshToken,
      userInfo: token.userInfo,
    } as TokenModel;

    localStorage.setItem('tokens', JSON.stringify(temp));

  }

  getToken() {
    return JSON.parse(localStorage.getItem('tokens')) as TokenModel;
  }

  setUser(user: User) {
    localStorage.setItem('userInfo', JSON.stringify(user));
    this.userProfile.next(user);
  }

  getUser() {
    return JSON.parse(localStorage.getItem('userInfo')) as User;
  }

  getAccessToken(): string {
    
    var localStorageToken = this.getToken();

    if (localStorageToken) {
      var isTokenExpired = this.jwtService.isTokenExpired(
        localStorageToken.accessToken
      );
      if (isTokenExpired) {
        this.userProfile.next(null);
        return '';
      }
      var localStorageUser = this.getUser();
      this.setUser(localStorageUser);
      return localStorageToken.accessToken;
    }
    return '';
  }

  userLogin(payload: LoginModel) {
    return this.httpClient
      .post(`${environment.apiUrl}/auth/logindriver`, payload)
      .pipe(
        map((data) => {
          var token = data as TokenModel;
          this.setToken(token);
          this.setUser(token.userInfo);          
          return true;
        }),
        catchError(this.formatErrors)
      );
  }

  userLogout() {
    localStorage.removeItem('tokens');
    localStorage.removeItem('userInfo');
    var userInfo = null;
    this.setUser(userInfo);
    return of(true);
  }

  userRegister(userRegister: UserRegister) {
    return this.httpClient
    .post(`${environment.apiUrl}/auth/register`, userRegister)
    .pipe(
      map((data) => {
        return true;
      }),
      catchError(this.formatErrors)
    );
  }

  refreshToken(payload: TokenModel) {    
    return this.httpClient
      .post(`${environment.apiUrl}/auth/refresh-token`, payload);
      // .pipe(
      //   map((data) => {
      //     var token = data as TokenModel;
      //     this.setToken(token);
      //     this.setUser(token.userInfo);
      //     return true;
      //   }),
      //   catchError(this.formatErrors)
      // );
    // return this.httpClient.post<TokenModel>(
    //   `${environment.apiUrl}/auth/refresh-token`,
    //   payload
    // );
  }

  canView(module: string, page: string): Observable<boolean> {
    return this.isGranted(module, page, PageAction.View);
  }

  canModify(module: string, page: string): Observable<boolean> {
    return this.isGranted(module, page, PageAction.Modify);
  }

  private isGranted(
    module: string,
    page: string,
    action: PageAction
  ): Observable<boolean> {
    return this.currentUser$.pipe(
      map((user) => {
        if (!user || user.authMenus.length === 0) {
          return false;
        }

        let granted = false;
        const authMenus = user.authMenus.filter((x) => x.module === module);

        if (authMenus.length > 0 && authMenus[0].pageName === '*') {
          granted =
            action === PageAction.View ? true : authMenus[0].action === action;
        } else {
          const authPage = authMenus.find((menu) => menu.pageName === page);
          granted =
            !!authPage &&
            (action === PageAction.View ? true : authPage.action === action);
        }
        return granted;
      })
    );
  }

  private formatErrors(response: any) {
    if (response instanceof HttpErrorResponse) {
      if (!(response.error instanceof Array)) {
        const errorMessage = {
          Messages: [response.error.errors.Messages],
        };

        return throwError(() => errorMessage);
      }
      
      const msgKeys = {
        Message: response.error.map((x) => x.message),
      };

      return throwError(() => msgKeys);
    }

    return throwError(() => response);
  }
}
