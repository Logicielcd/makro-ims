import { Injectable } from '@angular/core';
import { Supplier, SupplierGroup } from '@core/models/master/supplier.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class SupplierService {
  private apiController = 'supplier';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<Supplier[]> {    
    return this.apiService.get(`${this.apiController}`);    
  }

  getNewSupplier(): Observable<Supplier[]> {    
    return this.apiService.get(`${this.apiController}/newsupplier`);    
  }

  getPaged(paging): Observable<PagedResult> {    
    return this.apiService.post(
      `${this.apiController}/pages`,
      paging
    );       
  }
  
  getExport(paging): Observable<PagedResult> {    
    return this.apiService.post(
      `${this.apiController}/export`,
      paging
    );       
  }

  getBySupGroup(supGroupId: number): Observable<Supplier[]> {    
    return this.apiService.get(`${this.apiController}/bysupgroup/${supGroupId}`);    
  }

  getById(supplierCode: string): Observable<Supplier> {    
    return this.apiService.get(
      `${this.apiController}/${supplierCode}`
    );
  }

  getSupGroup(supGroupId: number): Observable<SupplierGroup> {    
    return this.apiService.get(`${this.apiController}/supgroup/${supGroupId}`);    
  }

  
  add(supplier: Supplier | Partial<Supplier>): Observable<Supplier> {    
    return this.apiService.post(`${this.apiController}`, {          
      ...supplier,
    });      
  }

  update(supplier: Supplier | Partial<Supplier>): Observable<Supplier> {    
    return this.apiService.post(`${this.apiController}/updatesup`, {          
      ...supplier,
    });      
  }

  updateSupContact(supContact: Supplier): Observable<Supplier> {    
    return this.apiService.post(`${this.apiController}/update`, {          
      ...supContact,
    });
  }

  
  getSupByUser(userName: string): Observable<Supplier[]> {    
    return this.apiService.get(`${this.apiController}/user`);    
  }


}
