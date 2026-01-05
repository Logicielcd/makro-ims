import { Injectable } from '@angular/core';
import { BookingHeader } from '@core/models/booking/booking-header.model';
import { PoList } from '@core/models/booking/po.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class PoService {
  private apiController = 'po';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getByPo(poNo: string,supCode: string,bookingDate: Date): Observable<PoList> {    
    return this.apiService.get(
      `${this.apiController}/${poNo}/${supCode}/${bookingDate.toJSON()}`
    );
  }

  add(bookingheader: BookingHeader | Partial<BookingHeader>): Observable<BookingHeader> {    
    return this.apiService.post(`${this.apiController}`, {          
      ...bookingheader,
    });      
  }

  update(bookingHeader: BookingHeader | Partial<BookingHeader>): Observable<BookingHeader> {    
    return this.apiService.put(`${this.apiController}`, {          
      ...bookingHeader,
    });
  }

  delete(bookingHeaderId: number) {    
    return this.apiService.delete(
      `${this.apiController}/${bookingHeaderId}`
    );      
  }

  getBySupCode(supCode: string,bookingDate: Date): Observable<PoList[]> {    
    return this.apiService.get(
      `${this.apiController}/getBySub/${supCode}/${bookingDate.toJSON()}`
    );
  }

}
