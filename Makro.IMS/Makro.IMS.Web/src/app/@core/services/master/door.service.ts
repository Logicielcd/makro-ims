import { Injectable } from '@angular/core';
import { Door } from '@core/models/master/door.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap, of } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class DoorService {
  private apiController = 'door';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getAll(): Observable<Door[]> {
    return this.apiService.get(`${this.apiController}`);
  }

  getByWarehouse(warehouseCode: string): Observable<Door[]> {
    return this.apiService.get(
      `${this.apiController}/warehouse/${warehouseCode}`
    );
  }

  getById(internalDoorId: number): Observable<Door> {
    return this.apiService.get(`${this.apiController}/${internalDoorId}`);
  }

  getByWarehouseAndSupCode(
    warehouseCode: string,
    supCode: string
  ): Observable<Door[]> {
    return this.apiService.get(
      `${this.apiController}/warehouse/${warehouseCode}/${supCode}`
    );
  }

  getByWarehouseAndOperation(
    warehouseCode: string,
    operationType: string,
    internalTruckCheckinId: number
  ): Observable<Door[]> {
    return this.apiService.get(
      `${this.apiController}/operation/${warehouseCode}/${operationType}/${internalTruckCheckinId}`
    );
  }

  getPaged(paging): Observable<PagedResult> {
    return this.apiService.post(`${this.apiController}/pages`, paging);
  }

  add(door: Door | Partial<Door>): Observable<Door> {
    return this.apiService.post(`${this.apiController}`, {
      ...door,
    });
  }

  update(door: Door | Partial<Door>): Observable<Door> {
    return this.apiService.post(`${this.apiController}/update`, {
      ...door,
    });
  }

  delete(doorId: number) {
    return this.apiService.post(`${this.apiController}/delete`, { doorId });
  }
}
