import { Injectable } from '@angular/core';
import { BookingTruckCheckIn, BookingHeader, BookingCheckOut, PreCheckIn, BookingPreCheckInDto, BookingCheckInDto, CheckIn, PoCheckInOut } from '@core/models/booking/booking-header.model';
import { BookingKey,BookingHeaderDto } from '@core/models/booking/booking-header.model';
import { BookingKeyDto } from '@core/models/booking/booking-header.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';
import { QueueManageSearchDto, QueueManageDto, DoorQueueDto, QueueSequence } from '@core/models/queuemanage/manage-queue.model';
import { QueueActionDto } from '@core/models/queuemanage/queue-action.model';

@Injectable({
  providedIn: 'root',
})
export class DriverService {
  private apiController = 'driver';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getGatePass(telNo: string): Observable<QueueManageDto[]> {    
    return this.apiService.get(
      `${this.apiController}/getgatepass/${telNo}`);
  }

  getTruckQueue(search: QueueManageSearchDto): Observable<QueueManageDto[]> {    
    return this.apiService.post(
      `${this.apiController}/truckqueue/`,{ 
        ...search,
      });
  }

}
