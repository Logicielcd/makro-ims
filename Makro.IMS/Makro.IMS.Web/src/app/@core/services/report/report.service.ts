import { Injectable } from '@angular/core';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';
import { ReportGatePass } from '@core/models/report/report-gatepass';
import { ReportSummaryBooking } from '@core/models/report/report-summarybooking';
import { ReportTruckStatus } from '@core/models/report/report-truckstatus';
import { ReportTransactionTrack } from '@core/models/report/report-transactiontrack';
import { ReportSlottimeBooking } from '@core/models/report/report-slottimebooking';
import { ReportDockDoorControl } from '@core/models/report/report-dockdoorcontrol';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private apiController = 'report';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  
  getGatePass(internalHeaderKey: number): Observable<ReportGatePass[]> {    
    return this.apiService.get(`${this.apiController}/gatepass/${internalHeaderKey}`);    
  }

  getSummaryBooking(warehouseCode: string,companyCode: string): Observable<ReportSummaryBooking> {    
    return this.apiService.get(`${this.apiController}/bookingsummary/${warehouseCode}/${companyCode}`);    
  }

  getTruckStatus(warehouseCode: string,companyCode: string,fromDate: Date,toDate: Date): Observable<ReportTruckStatus[]> {    
    return this.apiService.get(`${this.apiController}/truckstatus/${warehouseCode}/${companyCode}/${fromDate.toISOString()}/${toDate.toISOString()}`);
  }

  getTransactionTrack(warehouseCode: string,companyCode: string,fromDate: Date,toDate: Date): Observable<ReportTransactionTrack[]> {    
    return this.apiService.get(`${this.apiController}/transactiontrack/${warehouseCode}/${companyCode}/${fromDate.toISOString()}/${toDate.toISOString()}`);
  }

  getSlottimeBooking(warehouseCode: string,companyCode: string,fromDate: Date): Observable<ReportSlottimeBooking[]> {    
    return this.apiService.get(`${this.apiController}/slottimebooking/${warehouseCode}/${companyCode}/${fromDate.toISOString()}`);    
  }

  getSlottimeBookingActual(warehouseCode: string,companyCode: string,fromDate: Date): Observable<ReportSlottimeBooking[]> {    
    return this.apiService.get(`${this.apiController}/slottimebookingactual/${warehouseCode}/${companyCode}/${fromDate.toISOString()}`);    
  }

  getSlottimeBookingPending(warehouseCode: string,companyCode: string,fromDate: Date): Observable<ReportSlottimeBooking[]> {    
    return this.apiService.get(`${this.apiController}/slottimebookingpending/${warehouseCode}/${companyCode}/${fromDate.toISOString()}`);    
  }

  getDockDoorControl(warehouseCode: string,doorRange: string): Observable<ReportDockDoorControl[]> {    
    if(!doorRange){
      doorRange = 'ALL';
    }
    return this.apiService.get(`${this.apiController}/dockdoorcontrol/${warehouseCode}/${doorRange}`);    
  }

}
