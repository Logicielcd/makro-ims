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
import { DriverService } from '@core/services/driver/driver.service';
import { QueueManageDto } from '@core/models/queuemanage/manage-queue.model';

@UntilDestroy()
@Component({
  templateUrl: './gate-pass.component.html',
})
export class GatePassComponent implements OnInit {

  title = 'Gate Pass';
  cols: any[];
  gatePass: string;
  user: User;
  
  msgs: Message[] = [];

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  queueManages: QueueManageDto[] = [];
  bookingHeader: BookingHeaderDto;
  bookingTrucks: BookingTruckCheckIn[] = [];
  
  ref: DynamicDialogRef;
  
  qrCodeValue: string;
  queueValue: string;

  layout: string;
  
  visible: boolean = false;
  queuevisible: boolean = false;

  statuses: any[];

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    private warehouseService: WarehouseService,
    private authService: AuthService,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private driverService: DriverService,
    private route: ActivatedRoute,
  ) {
    this.statuses = [
      {
        value: "ALL",
        display: "All status"
      },
      {
        value: "CHECKIN",
        display: "Guard Check In"
      },
      {
        value: "QUEUE",
        display: "Assign Queue"
      },
      {
        value: "CALLTRUCK",
        display: "Assign Truck"
      },
      {
        value: "ONDOCK",
        display: "On Dock"
      },
      {
        value: "UNLOADING",
        display: "Start UnLoading"
      },
      {
        value: "UNLOADED",
        display: "Finish UnLoading"
      },
      {
        value: "LEAVEDOOR",
        display: "Leave Door"
      },
      {
        value: "SUBMITDOC",
        display: "Confirm document"
      },
      {
        value: "CHECKOUT",
        display: "Guard Check Out"
      }
    ];
  }

  ngOnInit() {
    this.user = this.authService.getUser();
    this.layout = 'grid';

    this.driverService.getGatePass(this.user.userName)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) => {
      console.log(data);
      this.queueManages = data;
      this.qrCodeValue = data[0].bookingId;
      this.queueManages.forEach((row)=>{
        row.statusDisplay = this.statuses.find(x=>x.value === row.status).display;
      })
      
    },error: (err) => {

    }});
    
  }

  showQr(qr:string) {
    this.qrCodeValue = qr;
    this.visible = true;
  }

  showQueue(queueNo:string) {
    this.queueValue = queueNo;
    this.queuevisible = true;
  }

  getSeverity (product: BookingTruckCheckIn) {
    switch (product.status) {
        case 'INSTOCK':
            return 'success';

        case 'LOWSTOCK':
            return 'warning';

        case 'OUTOFSTOCK':
            return 'danger';

        default:
            return null;
    }
  };

  refresh(): void {
    window.location.reload();
  }
  
}