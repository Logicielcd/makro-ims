import { Injectable } from '@angular/core';
import { WarehouseCapacity,WarehouseCapacityDto } from '@core/models/master/warehouse-capacity.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class WarehouseCapacityService {
  private apiController = 'warehouseCapacity';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<WarehouseCapacity[]> {    
    return this.apiService.get(`${this.apiController}`);    
  }

  getPaged(paging): Observable<PagedResult> {        
    return this.apiService.post(
      `${this.apiController}/pages`,
      paging
    );      
  }

  getById(warehouseCapacityId: number): Observable<WarehouseCapacity> {    
    return this.apiService.get(
      `${this.apiController}/${warehouseCapacityId}`
    );
  }

  add(warehousecapacity: WarehouseCapacity | Partial<WarehouseCapacity>): Observable<WarehouseCapacity> {    
    return this.apiService.post(`${this.apiController}`, {          
      ...warehousecapacity,
    });      
  }

  update(warehousecapacity: WarehouseCapacity | Partial<WarehouseCapacity>): Observable<WarehouseCapacity> {    
    return this.apiService.put(`${this.apiController}`, {          
      ...warehousecapacity,
    });
  }

  delete(warehouseCapacityId: number) {    
    return this.apiService.delete(
      `${this.apiController}/${warehouseCapacityId}`
    );      
  }

  getBookingCapacity(warehouseCode: string,bookingDate: Date): Observable<WarehouseCapacityDto[]> {    

    var addedHours = bookingDate.getTime() + (3600000*7);
    var newDate = new Date(addedHours);

    return this.apiService.get(
      `${this.apiController}/${newDate.toISOString()}/${warehouseCode}`
    );
  }

}
