import { Injectable } from '@angular/core';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';
import { Shunt } from '@core/models/master/shunt.model';

@Injectable({
  providedIn: 'root',
})
export class ShuntService {
  private apiController = 'shunt';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<Shunt[]> {
    return this.apiService.get(`${this.apiController}`);
  }

  getPaged(paging): Observable<PagedResult> {
    return this.apiService.post(`${this.apiController}/pages`, paging);
  }

  getById(shuntId: number): Observable<Shunt> {
    return this.apiService.get(`${this.apiController}/${shuntId}`);
  }

  add(warehouse: Shunt | Partial<Shunt>): Observable<Shunt> {
    return this.apiService.post(`${this.apiController}`, {
      ...warehouse,
    });
  }

  update(warehouse: Shunt | Partial<Shunt>): Observable<Shunt> {
    return this.apiService.put(`${this.apiController}`, {
      ...warehouse,
    });
  }

  delete(shunt: Shunt) {
    return this.apiService.post(`${this.apiController}/delete`, { ...shunt });
  }

  //-----------------------------Procress---------------------------------//
  get_by_status(status: string): Observable<Shunt[]> {
    return this.apiService.get(`${this.apiController}/status/${status}`);
  }
}
