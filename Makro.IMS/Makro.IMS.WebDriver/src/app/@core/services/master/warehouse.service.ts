import { Injectable } from '@angular/core';
import { Warehouse } from '@core/models/master/warehouse.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class WarehouseService {
  private apiController = 'warehouse';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<Warehouse[]> {    
    return this.apiService.get(`${this.apiController}`);    
  }

  getPaged(paging): Observable<PagedResult> {    
    return this.apiService.post(
      `${this.apiController}/pages`,
      paging
    );       
  }

  getById(warehouseCode: string): Observable<Warehouse> {    
    return this.apiService.get(
      `${this.apiController}/${warehouseCode}`
    );
  }

  add(warehouse: Warehouse | Partial<Warehouse>): Observable<Warehouse> {    
    return this.apiService.post(`${this.apiController}`, {          
      ...warehouse,
    });      
  }

  update(warehouse: Warehouse | Partial<Warehouse>): Observable<Warehouse> {    
    return this.apiService.put(`${this.apiController}`, {          
      ...warehouse,
    });      
  }

  delete(warehouseCode: string) {    
    return this.apiService.delete(
      `${this.apiController}/${warehouseCode}`
    );      
  }

}
