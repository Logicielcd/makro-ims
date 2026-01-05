import { Injectable } from '@angular/core';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { PoList } from '@core/models/booking/po.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';
import { SlotCapacity } from '@core/models/booking/slot-capacity';

@Injectable({
  providedIn: 'root',
})
export class SlotCapacityService {
  private apiController = 'slotcapacity';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getSlotCapacity(bookingDate: Date,warehouseCode: string,operationType: string): Observable<SlotCapacity[]> {    
    return this.apiService.get(
      `${this.apiController}/${bookingDate.toJSON()}/${warehouseCode}/${operationType}`
    );
  }

}
