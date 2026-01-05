import { Component,  OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { MenuItem, Message, MessageService,ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';

/* service */
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { GuardCheckInOutService } from '@core/services/booking/check-in.service';

/* model */
import { BookingTruckCheckIn, BookingHeaderDto } from '@core/models/booking/booking-header.model';
import { User } from '@core/models/account/user.model';
import { ActivatedRoute } from '@angular/router';
import { QRCodeModule } from 'angularx-qrcode';
import { QRCodeComponent } from 'angularx-qrcode';

@UntilDestroy()
@Component({
  templateUrl: './gate-pass.component.html',
})
export class GatePassComponent implements OnInit {

  title = 'Guard Check-In';
  cols: any[];
  gatePass: string;
  user: User;
  
  items: MenuItem[];
  msgs: Message[] = [];

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  bookingHeader: BookingHeaderDto;
  bookingTrucks: BookingTruckCheckIn[] = [];
  
  ref: DynamicDialogRef;
  
  qrCodeValue: string;

  // auth
  supReadonly: boolean = false;
  isSelected:boolean;

  @ViewChild('f') f: NgForm;

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    private warehouseService: WarehouseService,
    private bookingHeaderService: BookingHeaderService,    
    private authService: AuthService,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private guardCheckInOutService: GuardCheckInOutService,
    private route: ActivatedRoute,
  ) {
    
  }

  ngOnInit() {
    // Access the query parameters
    this.route.queryParams.subscribe(params => {
      const param1Value = params['A'];
      console.log('param1:', param1Value);
      this.qrCodeValue = 'CDC-FRESH-0001';
    });
  }

  

  refresh(): void {
    window.location.reload();
  }
  
}