import { Injectable } from '@angular/core';

import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';
import { InboundBookingDto } from '@core/models/booking/inbound-booking';

@Injectable({
  providedIn: 'root',
})

export class InboundBookingService {
  private apiController = 'inboundbooking';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}
  
  inboundBooking(inboundBooking: InboundBookingDto): Observable<any> {    
    return this.apiService.post(
      `${this.apiController}/inboundbooking/`,{ 
        ...inboundBooking,
      });
  }

}
