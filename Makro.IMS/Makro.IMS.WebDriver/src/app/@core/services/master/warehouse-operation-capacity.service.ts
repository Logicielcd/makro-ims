import { Injectable } from '@angular/core';
import { WarehouseOperationCapacity,WarehouseOperationCapacityDto } from '@core/models/master/warehouse-operation-capacity.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class WarehouseOperationCapacityService {
  private apiController = 'warehouseOperationCapacity';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<WarehouseOperationCapacity[]> {    
    return this.apiService.get(`${this.apiController}`);    
  }

  getPaged(paging): Observable<PagedResult> {        
    return this.apiService.post(
      `${this.apiController}/pages`,
      paging
    );      
  }

  getById(warehouseCapacityId: number): Observable<WarehouseOperationCapacity> {    
    return this.apiService.get(
      `${this.apiController}/${warehouseCapacityId}`
    );
  }

  add(warehousecapacity: WarehouseOperationCapacity | Partial<WarehouseOperationCapacity>): Observable<WarehouseOperationCapacity> {    
    return this.apiService.post(`${this.apiController}`, {          
      ...warehousecapacity,
    });      
  }

  update(warehousecapacity: WarehouseOperationCapacity | Partial<WarehouseOperationCapacity>): Observable<WarehouseOperationCapacity> {    
    return this.apiService.put(`${this.apiController}`, {          
      ...warehousecapacity,
    });
  }

  delete(warehouseCapacityId: number) {    
    return this.apiService.delete(
      `${this.apiController}/${warehouseCapacityId}`
    );      
  }

  getBookingCapacity(warehouseCode: string,bookingDate: Date): Observable<WarehouseOperationCapacityDto[]> {    

    var addedHours = bookingDate.getTime() + (3600000*7);
    var newDate = new Date(addedHours);

    return this.apiService.get(
      `${this.apiController}/${newDate.toISOString()}/${warehouseCode}`
    );
  }

}
