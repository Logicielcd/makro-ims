import { Injectable } from '@angular/core';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';
import { Trailer } from '@core/models/master/trailer.model';

@Injectable({
  providedIn: 'root',
})
export class TrailerService {
  private apiController = 'trailer';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<Trailer[]> {
    return this.apiService.get(`${this.apiController}`);
  }

  getPaged(paging): Observable<PagedResult> {
    return this.apiService.post(`${this.apiController}/pages`, paging);
  }

  getById(trailerId: number): Observable<Trailer> {
    return this.apiService.get(`${this.apiController}/${trailerId}`);
  }

  add(trailer: Trailer | Partial<Trailer>): Observable<Trailer> {
    return this.apiService.post(`${this.apiController}`, {
      ...trailer,
    });
  }

  update(trailer: Trailer | Partial<Trailer>): Observable<Trailer> {
    return this.apiService.put(`${this.apiController}`, {
      ...trailer,
    });
  }

  delete(trailer: Trailer) {
    return this.apiService.post(`${this.apiController}/delete`, { ...trailer });
  }
}
