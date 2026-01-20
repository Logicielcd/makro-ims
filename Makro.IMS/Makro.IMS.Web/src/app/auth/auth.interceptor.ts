import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { TokenModel } from '@core/models/auth/token-model';
import { Observable, switchMap, throwError,map } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  private _isoDateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d*)?Z$/;

  constructor(
    private jwtHelper: JwtHelperService,
    private authService: AuthService,
    public router: Router
  ) {}
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {

    if(!this.validateTimeZone()){
      this.router.navigate(['/auth/login']);
    }
    // req = req.clone({
    //   headers: req.headers.set('rejectUnauthorized', 'false'),
    // });

    if (
      req.url.indexOf('login') > -1 ||
      req.url.indexOf('refresh-token') > -1 ||
      req.url.indexOf('register') > -1 ||
      req.url.indexOf('validate-token') > -1
    ) {
      return next.handle(req).pipe(map( (val: HttpEvent<any>) => {
        if (val instanceof HttpResponse){
          const body = val.body;
          // console.log(body);
          this.convert(body);
        }
        return val;
      }));
    }

    req = req.clone({
      headers: req.headers.set('Content-Type', 'application/json'),
    });

    const localStorageTokens = this.authService.getToken();

    if (localStorageTokens) {
      var isTokenExpired = this.jwtHelper.isTokenExpired(
        localStorageTokens.accessToken
      );

      //console.log('Check token : ' + localStorageTokens.accessToken);

      if (!isTokenExpired) {
        const transformedReq = req.clone({
          headers: req.headers.set(
            'Authorization',
            `bearer ${localStorageTokens.accessToken}`
          ),
        });

        return this.authService.validateToken(localStorageTokens).pipe(
          switchMap((result: any) => {            
            if(result == false){
              this.router.navigate(['/auth/login/duplicate']);
              return throwError(() => 'Duplicate Login');
            }
            // this.authService.setToken(newTokens);
            // this.authService.setUser(newTokens.userInfo);
            // const transformedReq = req.clone({
            //   headers: req.headers.set(
            //     'Authorization',
            //     `bearer ${newTokens.accessToken}`
            //   ),
            // });
            return next.handle(transformedReq).pipe(map( (val: HttpEvent<any>) => {
              if (val instanceof HttpResponse){
                const body = val.body;
                this.convert(body);
              }
              return val;
            }));
          })
        );

        // validate token
        // this.authService.validateToken(localStorageTokens).pipe(
        //   switchMap((result: any) =>{
        //     console.log(result);
            
        //     return next.handle(transformedReq).pipe(map( (val: HttpEvent<any>) => {
        //       if (val instanceof HttpResponse){
        //         const body = val.body;
        //         this.convert(body);
        //       }
        //       return val;
        //     }));
        //   })
        // );

        // this.authService.refreshToken(localStorageTokens).pipe(
        //   switchMap((newTokens: TokenModel) => {
        //     this.authService.setToken(newTokens);
        //     this.authService.setUser(newTokens.userInfo);
        //     const transformedReq = req.clone({
        //       headers: req.headers.set(
        //         'Authorization',
        //         `bearer ${newTokens.accessToken}`
        //       ),
        //     });
        //     return next.handle(transformedReq).pipe(map( (val: HttpEvent<any>) => {
        //       if (val instanceof HttpResponse){
        //         const body = val.body;
        //         this.convert(body);
        //       }
        //       return val;
        //     }));
        //   })
        // );

        // return next.handle(transformedReq).pipe(map( (val: HttpEvent<any>) => {
        //   if (val instanceof HttpResponse){
        //     const body = val.body;
        //     this.convert(body);
        //   }
        //   return val;
        // }));
      } else {
        return this.authService.refreshToken(localStorageTokens).pipe(
          switchMap((newTokens: TokenModel) => {
            this.authService.setToken(newTokens);
            this.authService.setUser(newTokens.userInfo);
            const transformedReq = req.clone({
              headers: req.headers.set(
                'Authorization',
                `bearer ${newTokens.accessToken}`
              ),
            });
            return next.handle(transformedReq).pipe(map( (val: HttpEvent<any>) => {
              if (val instanceof HttpResponse){
                const body = val.body;
                this.convert(body);
              }
              return val;
            }));
          })
        );
      }
    }

    this.router.navigate(['/auth/login']);
    return throwError(() => 'Invalid call');
  }

  isIsoDateString(value: any): boolean {
    if (value === null || value === undefined) {
      return false;
    }
    if (typeof value === 'string'){
      return this._isoDateFormat.test(value);
    }    return false;
  }

  convert(body: any){
    if (body === null || body === undefined ) {
      return body;
    }
    if (typeof body !== 'object' ){
      return body;
    }
    for (const key of Object.keys(body)) {
      const value = body[key];
      if (this.isIsoDateString(value)) {        
        body[key] = new Date(value);
      } else if (typeof value === 'object') {
        this.convert(value);
      }
    }
  }

  validateTimeZone():boolean{
    const clientOffset = new Date().getTimezoneOffset(); // ค่า offset ของ client ในหน่วยนาที
    const requiredOffset = -420; // Offset -420 หมายถึง UTC+7
  
    // ตรวจสอบว่า timezone ของ client ตรงกับที่ต้องการหรือไม่
    if (clientOffset !== requiredOffset) {
      // ถ้า timezone ไม่ตรงกัน ให้ redirect ไปหน้า login
      //window.location.href = "/login"; // ระบุ URL ของหน้า login
      return false;
    }
    return true;
  }
}
