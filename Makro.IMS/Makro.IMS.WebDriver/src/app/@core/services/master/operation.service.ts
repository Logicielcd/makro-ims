import { Injectable } from '@angular/core';
import { Door } from '@core/models/master/door.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap, of } from 'rxjs';
import { ApiService } from '../api.service';
import { Operation } from '@core/models/master/operation.model';

@Injectable({
  providedIn: 'root',
})
export class OperationService {
  private apiController = 'operation';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<Operation[]> {    
    return this.apiService.get(`${this.apiController}`);   
  }

  getByWarehouse(warehouseCode: string): Observable<Operation[]> {    
    return this.apiService.get(`${this.apiController}/${warehouseCode}`);    
  }

  getByOperationNameAndWhse(operationName: string,warehouseCode: string): Observable<Operation> {    
    return this.apiService.get(`${this.apiController}/${operationName}/${warehouseCode}`);    
  }

  getPaged(paging): Observable<PagedResult> {    
    return this.apiService.post(
      `${this.apiController}/pages`,
      paging
    );       
  }

  getById(operationName: string): Observable<Operation> {    
    return this.apiService.get(
      `${this.apiController}/${operationName}`
    );
  }

  add(operation: Operation | Partial<Operation>): Observable<Operation> {    
    return this.apiService.post(`${this.apiController}`, {          
      ...operation,
    });      
  }

  update(operation: Operation | Partial<Operation>): Observable<Operation> {    
    return this.apiService.put(`${this.apiController}`, {          
      ...operation,
    });      
  }

  delete(operationName: string) {    
    return this.apiService.delete(
      `${this.apiController}/${operationName}`
    );      
  }



}
