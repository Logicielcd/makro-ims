import { Injectable } from '@angular/core';
import { Door } from '@core/models/master/door.model';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap, of } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class TruckMasterService {
  private apiController = 'truckMaster';

  constructor(
    private apiService: ApiService
  ) {}

  getAll(): Observable<TruckMaster[]> {    
    return this.apiService.get(`${this.apiController}`);   
  }

}
