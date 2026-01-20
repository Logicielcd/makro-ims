import { Injectable } from '@angular/core';
import { Supplier, SupplierGroup } from '@core/models/master/supplier.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class SupplierGroupService {
  private apiController = 'suppliergroup';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<SupplierGroup[]> {    
    return this.apiService.get(`${this.apiController}`);    
  }

  getPaged(paging): Observable<PagedResult> {    
    return this.apiService.post(
      `${this.apiController}/pages`,
      paging
    );       
  }
  
  getBySupGroup(supGroupId: number): Observable<SupplierGroup[]> {    
    return this.apiService.get(`${this.apiController}/bysupgroup/${supGroupId}`);    
  }

  getById(internalSubGroupId: number): Observable<SupplierGroup> {    
    return this.apiService.get(
      `${this.apiController}/${internalSubGroupId}`
    );
  }

  getSupGroup(supGroupId: number): Observable<SupplierGroup> {    
    return this.apiService.get(`${this.apiController}/supgroup/${supGroupId}`);    
  }

  add(supGroup: SupplierGroup | Partial<SupplierGroup>): Observable<SupplierGroup> {    
    return this.apiService.post(`${this.apiController}`, {          
      ...supGroup,
    });      
  }

  updateSupGroup(supGroup: SupplierGroup): Observable<SupplierGroup> {    
    return this.apiService.post(`${this.apiController}/update`, {          
      ...supGroup,
    });
  }


}
