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
export class ManageQueueService {
  private apiController = 'managequeue';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  getManageQueue(search: QueueManageSearchDto): Observable<QueueManageDto[]> {    
    return this.apiService.post(
      `${this.apiController}/managequeue/`,{ 
        ...search,
      });
  }

  getTruckQueue(search: QueueManageSearchDto): Observable<QueueManageDto[]> {    
    return this.apiService.post(
      `${this.apiController}/truckqueue/`,{ 
        ...search,
      });
  }

  getDoorQueue(warehouseCode: string): Observable<DoorQueueDto[]> {    
    return this.apiService.get(`${this.apiController}/queuedoor/${warehouseCode}`);    
  }

  getDoorQueueByOperation(warehouseCode: string,operationType: string): Observable<DoorQueueDto[]> {    
    return this.apiService.get(`${this.apiController}/queuedoorbyoperation/${warehouseCode}/${operationType}`);
  }

  getQueueSequence(warehouseCode: string,operationType: string): Observable<QueueSequence> {    
    return this.apiService.get(`${this.apiController}/queuesequence/${warehouseCode}/${operationType}`);    
  }

  createQueue(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/createqueue/`,{ 
        ...action,
      });
  }

  assignDoor(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/assigndoor/`,{ 
        ...action,
      });
  }

  changeDoor(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/changedoor/`,{ 
        ...action,
      });
  }

  truckOnDoor(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/truckondoor/`,{ 
        ...action,
      });
  }

  startUnloading(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/startunloading/`,{ 
        ...action,
      });
  }

  finishUnloading(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/finishunloading/`,{ 
        ...action,
      });
  }

  leaveDoor(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/leaveDoor/`,{ 
        ...action,
      });
  }

  sendDocument(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/senddocument/`,{ 
        ...action,
      });
  }

  unloadFinish(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/unloadfinish/`,{ 
        ...action,
      });
  }

  getPoCheckin(internalTruckCheckInId: number): Observable<PoCheckInOut[]>{
    return this.apiService.get(`${this.apiController}/pocheckin/${internalTruckCheckInId}`);    
  }

  documentCheckInPo(checkIn: CheckIn): Observable<PoCheckInOut[]> {    
    return this.apiService.post(
      `${this.apiController}/documentcheckInPo/`,{ 
        ...checkIn,
      });
  }

  documentCheckInPoNotInBooking(checkIn: CheckIn): Observable<PoCheckInOut[]> {    
    return this.apiService.post(
      `${this.apiController}/documentcheckinponotinbooking/`,{ 
        ...checkIn,
      });
  }

  deleteCheckInPo(checkIn: CheckIn): Observable<PoCheckInOut[]>{
    return this.apiService.post(
      `${this.apiController}/deletecheckinpo/`,{ 
        ...checkIn,
      });
  }

  getPoCheckout(internalTruckCheckInId: number): Observable<PoCheckInOut[]>{
    return this.apiService.get(`${this.apiController}/pocheckout/${internalTruckCheckInId}`);    
  }

  documentCheckOutPo(checkIn: CheckIn): Observable<PoCheckInOut[]> {    
    return this.apiService.post(
      `${this.apiController}/documentcheckOutPo/`,{ 
        ...checkIn,
      });
  }

  deleteCheckOutPo(checkIn: CheckIn): Observable<PoCheckInOut[]>{
    return this.apiService.post(
      `${this.apiController}/deletecheckoutpo/`,{ 
        ...checkIn,
      });
  }

  sendSms(action: QueueManageDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/sendsms/`,{ 
        ...action,
      });
  }

  /*
  getAll(): Observable<BookingHeader[]> {    
    return this.apiService.get(`${this.apiController}`);    
  }

  getPaged(paging): Observable<PagedResult> {        
    return this.apiService.get(
      `${this.apiController}/pages`,
      paging
    );      
  }

  getPreCheckInPaged(paging): Observable<PagedResult> {        
    return this.apiService.get(
      `${this.apiController}/precheckinpages`,
      paging
    );      
  }

  getPreCheckInCompletedPaged(paging): Observable<PagedResult> {        
    return this.apiService.get(
      `${this.apiController}/precheckincompletedpages`,
      paging
    );      
  }

  getPreCheckInExportPaged(paging): Observable<PagedResult> {        
    return this.apiService.get(
      `${this.apiController}/precheckinpagesexport`,
      paging
    );      
  }

  getDashboard(warehouseCode: string): Observable<BookingHeader[]> {    
    return this.apiService.get(`${this.apiController}/dashboard/${warehouseCode}`);    
  }

  getById(bookingId: number): Observable<BookingHeader> {    
    return this.apiService.get(
      `${this.apiController}/${bookingId}`
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


  getPoCheckIn(supCode:string,warehouseCode:string): Observable<BookingCheckInDto[]>{
    return this.apiService.get(
      `${this.apiController}/getPoCheckIn/${supCode}/${warehouseCode}`
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

  checkInPo(checkIn: CheckIn): Observable<BookingCheckInDto[]> {    
    return this.apiService.post(
      `${this.apiController}/checkInPo/`,{ 
        ...checkIn,
      });
  }

  checkInBooking(checkIn: CheckIn): Observable<BookingCheckInDto[]> {    
    return this.apiService.post(
      `${this.apiController}/checkInBooking/`,{ 
        ...checkIn,
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
