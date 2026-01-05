import { Injectable } from '@angular/core';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { Trailer } from '@core/models/master/trailer.model';

@Injectable({
  providedIn: 'root',
})
export class JobTransferService {
  private apiController = 'job';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getPaged(paging): Observable<PagedResult> {
    return this.apiService.post(`${this.apiController}/pages`, paging);
  }

  getAll(): Observable<any[]> {
    return this.apiService.get(`${this.apiController}`);
  }

  get_by_on_dock(): Observable<any[]> {
    return this.apiService.get(`${this.apiController}/onDock`);
  }

  getById(shuntId: number): Observable<any> {
    return this.apiService.get(`${this.apiController}/${shuntId}`);
  }

  create_job(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}`, {
      ...job,
    });
  }

  confirm_assign_job(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/confirmAssign`, {
      ...job,
    });
  }

  confirm_start_job(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/confirmStart`, {
      ...job,
    });
  }

  confirm_complete_job(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/confirmComplete`, {
      ...job,
    });
  }

  cancel_job(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/cancelJob`, {
      ...job,
    });
  }

  update_in_dc(trailer: any | Partial<any>) {
    return this.apiService.post(`${this.apiController}/updateInDc`, {
      ...trailer,
    });
  }

  update_out_dc(trailer: any | Partial<any>) {
    return this.apiService.post(`${this.apiController}/updateOutDc`, {
      ...trailer,
    });
  }
}
