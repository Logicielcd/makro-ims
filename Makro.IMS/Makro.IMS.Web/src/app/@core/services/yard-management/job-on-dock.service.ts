import { Injectable } from '@angular/core';
import { AuthService } from 'auth/auth.service';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class JobOnDockService {
  private apiController = 'jobOnDock';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<any[]> {
    return this.apiService.get(`${this.apiController}`);
  }

  changeDoor(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/changeDoor`, {
      ...job,
    });
  }

  confirmLoading(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/confirmLoading`, {
      ...job,
    });
  }

  confirmLoaded(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/confirmLoaded`, {
      ...job,
    });
  }

  confirmComplete(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/confirmComplete`, {
      ...job,
    });
  }

  create_job_change_trailer(job: any | Partial<any>): Observable<any> {
    return this.apiService.post(`${this.apiController}/createJobChangeTrailer`, {
      ...job,
    });
  }
}
