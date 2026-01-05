import { Injectable } from '@angular/core';
import { Door } from '@core/models/master/door.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap, of } from 'rxjs';
import { ApiService } from '../api.service';
import { Operation } from '@core/models/master/operation.model';
import { OperationTime } from '@core/models/master/operation-time.model';
import { OperationCapacity } from '@core/models/master/operation-capacity.model';
import { OperationFixSlot } from '@core/models/master/operation-fix-slot.model';

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

  getOperationTimeByWhseAndOperation(warehouseCode:string, operationType: string): Observable<OperationTime[]> {
    return this.apiService.get(`${this.apiController}/operationtime/${operationType}/${warehouseCode}`)
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
    return this.apiService.post(`${this.apiController}/update`, {          
      ...operation,
    });      
  }

  delete(operation: Operation) {    
    return this.apiService.post(
      `${this.apiController}/delete`,{...operation,}
    );      
  }

  addOperationTime(operationTime: OperationTime | Partial<OperationTime>): Observable<OperationTime> {    
    return this.apiService.post(`${this.apiController}/operationtime`, {          
      ...operationTime,
    });      
  }

  deleteOperationTime(operationTime: OperationTime) {    
    return this.apiService.post(
      `${this.apiController}/delete/operationTime`, {
        ...operationTime
      });      
  }

  getOperationCap(operationType: string,warehouseCode: string): Observable<OperationCapacity[]>{
    return this.apiService.get(`${this.apiController}/operationcapacity/${operationType}/${warehouseCode}`)
  }

  saveOperationCap(operationCap: OperationCapacity | Partial<OperationCapacity>){
    return this.apiService.post(`${this.apiController}/operationcapacity`, {          
      ...operationCap,
    });  
  }

  getOperationFixSlot(operationType: string,warehouseCode: string): Observable<OperationFixSlot[]>{
    return this.apiService.get(`${this.apiController}/operationfixslot/${operationType}/${warehouseCode}`)
  }

  getOperationFixSlotBySupplierGroup(operationType: string,warehouseCode: string,supGroup: number): Observable<OperationFixSlot[]>{
    return this.apiService.get(`${this.apiController}/operationfixslot/${operationType}/${warehouseCode}/${supGroup}`)
  }

  saveOperationFixSlot(operationFixSlot: OperationFixSlot | Partial<OperationFixSlot>){
    return this.apiService.post(`${this.apiController}/operationfixslot`, {          
      ...operationFixSlot,
    });  
  }

  deleteOperationFixSlot(operationFixSlot: OperationFixSlot) {    
    return this.apiService.post(
      `${this.apiController}/delete/operationfixslot`, {
        ...operationFixSlot
      });      
  }


}
