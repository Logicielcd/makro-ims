import { Injectable } from '@angular/core';
import { BookingKeyDto } from '@core/models/booking/booking-header.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class BookingKeyService {
  private apiController = 'bookingKey';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  add(bookingKey: BookingKeyDto | Partial<any>): Observable<any> {    
    return this.apiService.post(`${this.apiController}`, bookingKey);      
  }

  update(bookingKey: BookingKeyDto | Partial<string[]>): Observable<string[]> {    
    return this.apiService.post(`${this.apiController}/updateBooking`, bookingKey);      
  }

  getBookingKey(bookingKeyId: number): Observable<BookingKeyDto>{
    return this.apiService.get(`${this.apiController}/${bookingKeyId}`)
  }
}
