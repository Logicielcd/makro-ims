import { Injectable } from '@angular/core';
import { CreateBookingExcelDto } from '@core/models/booking/booking-create-excel.model';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class CreateBookingExcelService {
  private apiController = 'CreateBookingExcel';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  importExcel(createBookingExcel: CreateBookingExcelDto | Partial<CreateBookingExcelDto>): Observable<CreateBookingExcelDto> {    
    return this.apiService.post(`${this.apiController}/import`, {          
      ...createBookingExcel,
    });      
  }

}
