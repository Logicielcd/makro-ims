import { Injectable } from '@angular/core';
import { AuthService } from 'auth/auth.service';
import { Observable, switchMap } from 'rxjs';
import { ApiService } from '../api.service';
import { PreCheckInExcelDetailDto, PreCheckInExcelDto } from '@core/models/booking/pre-check-in-excel.model';

@Injectable({
  providedIn: 'root',
})
export class PreCheckInExcelService {
  private apiController = 'PreCheckInExcel';

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  importExcel(preCheckInExcel: PreCheckInExcelDto | Partial<PreCheckInExcelDto>): Observable<PreCheckInExcelDto> {    
    return this.apiService.post(`${this.apiController}/import`, {          
      ...preCheckInExcel,
    });      
  }

}
