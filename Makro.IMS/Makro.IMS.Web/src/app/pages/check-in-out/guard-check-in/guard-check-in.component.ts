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

/*  */
import { DialogComponent } from './dialog/dialog.component';
import { GuardCheckInOutDto } from '@core/models/booking/check-in-out.model';

@UntilDestroy()
@Component({
  templateUrl: './guard-check-in.component.html',
})
export class GuardCheckInComponent implements OnInit {

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
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

    this.user = this.authService.getUser();
  }

  ngOnInit() {
    this.isSelected = false;
    this.user = this.authService.getUser();   

    this.cols = [
      {
        field: 'truckType',
        display: 'string',        
        header: 'Truck Type',
      },
      {
        field: 'licensePlate',
        display: 'string',
        header: 'License Plate',
      },
      {
        field: 'driverName',
        display: 'string',        
        header: 'Driver Name',
      },
    ];
  }

  onGatePass(){
    this.guardCheckInOutService.getGuardCheckIn(this.gatePass)
    .pipe(untilDestroyed(this))
    .subscribe({next:(data) =>{
      this.bookingHeader = data;
      this.bookingTrucks = data.bookingCheckIns.slice(0,1);
      
      this.ref = this.dialogService.open(DialogComponent, {
        header: 'Gate Pass',
        width: '800px',
        contentStyle: {"max-height": "1200px", "min-height" : "500px", "overflow": "auto"},        
        baseZIndex: 1000,
        data: {
          bookingHeader: data,
        }
      });

      this.ref.onClose.subscribe((ret: any) =>{
        
        if (ret == true)
        {
          this.refresh();                   
        }
    });


    },
    error:(error) => {
      this.msgs = [];
            error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
    }});
  }

  refresh(): void {
    window.location.reload();
  }
  
}