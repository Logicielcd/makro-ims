import { Injectable } from '@angular/core';
import { BookingTruckCheckIn, BookingHeader, BookingCheckOut, PreCheckIn, BookingPreCheckInDto, BookingCheckInDto, CheckIn, PoCheckInOut, BookingTruck } from '@core/models/booking/booking-header.model';
import { BookingKey,BookingHeaderDto } from '@core/models/booking/booking-header.model';
import { BookingKeyDto } from '@core/models/booking/booking-header.model';
import { PagedResult } from '@core/models/page-result.model';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';
import { QueueManageSearchDto, QueueManageDto, DoorQueueDto, QueueSequence } from '@core/models/queuemanage/manage-queue.model';
import { QueueActionDto } from '@core/models/queuemanage/queue-action.model';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';

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

  cancelDoor(action: QueueActionDto): Observable<boolean> {    
    return this.apiService.post(
      `${this.apiController}/canceldoor/`,{ 
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

  deleteUnloadPo(checkIn: CheckIn): Observable<PoCheckInOut[]>{
    return this.apiService.post(
      `${this.apiController}/deleteuploadpo/`,{ 
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

  getBookingTruckCheckIn(internalTruckCheckInId: number):Observable<BookingTruckCheckIn>{
    return this.apiService.get(`${this.apiController}/bookingtruckcheckin/${internalTruckCheckInId}`);
  }

  updateDriverInfo(bookingtruckcheckin: BookingTruckCheckIn): Observable<boolean>{
    return this.apiService.post(`${this.apiController}/updatedriverinfo/`,{ ...bookingtruckcheckin,});
  }

}
