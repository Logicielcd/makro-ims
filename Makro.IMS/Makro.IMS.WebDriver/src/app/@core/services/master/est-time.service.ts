import { Injectable } from '@angular/core';
import { Door } from '@core/models/master/door.model';
import { EstTime } from '@core/models/master/est-time.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap, of } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class EstTimeService {
  private apiController = 'estTime';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<EstTime[]> {    
    return this.apiService.get(`${this.apiController}`);   
  }

  getBySupGroupId(supGroupId: number): Observable<EstTime[]> {    
    return this.apiService.get(`${this.apiController}/supGroupId/${supGroupId}`);    
  }

}
