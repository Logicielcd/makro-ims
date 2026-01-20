import { Injectable } from '@angular/core';
import { Trailer } from '@core/models/master/trailer.model';
import { Yard } from '@core/models/master/yard.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class YardService {
  private apiController = 'yard';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<Yard[]> {
    return this.apiService.get(`${this.apiController}`);
  }

  getPaged(paging): Observable<PagedResult> {
    return this.apiService.post(`${this.apiController}/pages`, paging);
  }

  get_yard_monitor(): Observable<any[]> {
    return this.apiService.get(`${this.apiController}/yardMonitor`);
  }

  getById(yardId: number): Observable<Yard> {
    return this.apiService.get(`${this.apiController}/${yardId}`);
  }

  add(yard: Trailer | Partial<Yard>): Observable<Yard> {
    return this.apiService.post(`${this.apiController}`, {
      ...yard,
    });
  }

  update(yard: Trailer | Partial<Yard>): Observable<Yard> {
    return this.apiService.put(`${this.apiController}`, {
      ...yard,
    });
  }

  delete(yard: Yard) {
    return this.apiService.post(`${this.apiController}/delete`, { ...yard });
  }
  
  get_by_yardType(yardType: string): Observable<Yard[]> {
    return this.apiService.get(`${this.apiController}/yardType/${yardType}`);
  }
}
