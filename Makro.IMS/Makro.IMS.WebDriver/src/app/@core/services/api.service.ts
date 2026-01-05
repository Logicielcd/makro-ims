import {
  HttpClient,
  HttpErrorResponse,
  HttpParams
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}

  get(path: string, params: HttpParams = new HttpParams()): Observable<any> {
    return this.http
      .get(`${environment.apiUrl}/${path}`, { params })
      .pipe(catchError(this.formatErrors));
  }

  put(path: string, body: object = {}): Observable<any> {
    return this.http
      .put(`${environment.apiUrl}/${path}`, JSON.stringify(body))
      .pipe(catchError(this.formatErrors));
  }

  post(path: string, body: object = {}): Observable<any> {
    return this.http
      .post(`${environment.apiUrl}/${path}`, JSON.stringify(body))
      .pipe(catchError(this.formatErrors));
  }

  postId(path: string, id: number): Observable<any> {
    return this.http
      .post(`${environment.apiUrl}/${path}`, id)
      .pipe(catchError(this.formatErrors));
  }


  delete(path: string): Observable<any> {
    return this.http
      .delete(`${environment.apiUrl}/${path}`)
      .pipe(catchError(this.formatErrors));
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
