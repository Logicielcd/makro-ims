import { Injectable } from '@angular/core';
import { BookingTruckCheckIn, BookingHeader, BookingCheckOut, PreCheckIn, BookingPreCheckInDto, BookingCheckInDto, CheckIn } from '@core/models/booking/booking-header.model';
import { BookingKey,BookingHeaderDto } from '@core/models/booking/booking-header.model';
import { BookingKeyDto } from '@core/models/booking/booking-header.model';
import { GuardCheckInOutDto } from '@core/models/booking/check-in-out.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})

export class GuardCheckInOutService {
  private apiController = 'guardcheckinout';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}
  
  getPoCheckIn(supCode:string,warehouseCode:string): Observable<BookingCheckInDto[]>{
    return this.apiService.get(
      `${this.apiController}/getPoCheckIn/${supCode}/${warehouseCode}`
    );
  }

  guardCheckInBooking(checkIn: GuardCheckInOutDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/guardCheckinBooking/`,{ 
        ...checkIn,
      });
  }

  guardCheckInByTruck(checkIn: GuardCheckInOutDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/guardCheckinByTruck/`,{ 
        ...checkIn,
      });
  }

  guardCheckOutByTruck(checkIn: GuardCheckInOutDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/guardCheckoutByTruck/`,{ 
        ...checkIn,
      });
  }

  documentCheckInPo(checkIn: CheckIn): Observable<BookingCheckInDto[]> {    
    return this.apiService.post(
      `${this.apiController}/documentcheckInPo/`,{ 
        ...checkIn,
      });
  }

  documentCheckInBooking(checkIn: CheckIn): Observable<BookingCheckInDto[]> {    
    return this.apiService.post(
      `${this.apiController}/documentcheckInBooking/`,{ 
        ...checkIn,
      });
  }

  getGuardCheckIn(bookingId: string): Observable<BookingHeaderDto>{
    return this.apiService.get(
      `${this.apiController}/getGuardCheckIn/${bookingId}`);
  }

  getGuardCheckOut(bookingId: string): Observable<BookingHeaderDto>{
    return this.apiService.get(
      `${this.apiController}/getGuardCheckOut/${bookingId}`);
  }


  /*
  getPreCheckInExportPaged(paging): Observable<PagedResult> {        
    return this.apiService.get(
      `${this.apiController}/precheckinpagesexport`,
      paging
    );      
  }


  getByPo(poNo: string,supCode: string): Observable<BookingHeaderDto> {    
    return this.apiService.get(
      `${this.apiController}/getByPo/${poNo}/${supCode}`
    );
  }

  getBookingByBookingId(bookingId: string): Observable<BookingHeaderDto> {    
    return this.apiService.get(
      `${this.apiController}/getBookingByBooking/${bookingId}`
    );
  }

  getByBookingId(bookingId: string,supCode: string): Observable<BookingHeaderDto> {    
    return this.apiService.get(
      `${this.apiController}/getByBooking/${bookingId}/${supCode}`
    );
  }

  getByBookingHeaderKey(bookingHeaderKey: number,supCode: string): Observable<BookingHeaderDto> {    
    return this.apiService.get(
      `${this.apiController}/getByBookingHeaderKey/${bookingHeaderKey}/${supCode}`
    );
  }

  getByBookingHeaderBySupCodeAndBookingDate(supCode: string,bookingDate: Date): Observable<BookingHeader[]> {    
    return this.apiService.get(
      `${this.apiController}/getBySupAndBookingDate/${supCode}/${bookingDate.toJSON()}`
    );
  }


  getPoCheckOut(poNo: string,supCode: string): Observable<BookingHeaderDto> {    
    return this.apiService.get(
      `${this.apiController}/getPoCheckOut/${poNo}/${supCode}`
    );
  }

  saveCheckIn(bookingCheckIn: BookingTruckCheckIn): Observable<BookingTruckCheckIn[]> {    
    return this.apiService.post(
      `${this.apiController}/checkIn/`,{ 
        ...bookingCheckIn,
      });
  }

  

  savePreCheckIn(bookingCheckIn: PreCheckIn): Observable<BookingHeaderDto> {    
    return this.apiService.post(
      `${this.apiController}/precheckin/`,{ 
        ...bookingCheckIn,
      });
  }

  saveCheckOut(bookingCheckIn: BookingTruckCheckIn): Observable<BookingHeaderDto> {    
    return this.apiService.post(
      `${this.apiController}/checkOut/`,{ 
        ...bookingCheckIn,
      });
  }

  saveCheckOutPo(bookingCheckOut: BookingCheckOut): Observable<BookingHeaderDto> {    
    return this.apiService.post(
      `${this.apiController}/checkOut/`,{ 
        ...bookingCheckOut,
      });
  }

  add(bookingKey: BookingKeyDto | Partial<BookingKeyDto>): Observable<BookingKeyDto> {    
    return this.apiService.post(`${this.apiController}/savebooking/`, {          
      ...bookingKey,
    });      
  }

  update(bookingHeader: BookingHeader | Partial<BookingHeader>): Observable<BookingHeader> {    
    return this.apiService.put(`${this.apiController}`, {          
      ...bookingHeader,
    });
  }

  delete(bookingHeaderId: number): Observable<boolean> {    
    return this.apiService.postId(
      `${this.apiController}/deletebookingheader/`,
       bookingHeaderId
    );      
  }

  deleteall(bookingHeaderId: number): Observable<boolean> {    
    return this.apiService.postId(
      `${this.apiController}/deletebookingheaderall/`,
       bookingHeaderId
    );      
  }

  deletePreCheckIn(bookingHeaderId: number): Observable<boolean> {    
    return this.apiService.postId(
      `${this.apiController}/deleteprecheckin/`,
       bookingHeaderId
    );      
  }

  approved(bookingHeaderId: number): Observable<boolean> {    
    return this.apiService.postId(
      `${this.apiController}/approvebooking/`,
       bookingHeaderId
    );      
  }
  */

}
