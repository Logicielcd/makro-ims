import { Component, NgModule, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AuthService } from 'auth/auth.service';
import { PoService } from '@core/services/booking/po.service';

import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { concatMap, map, Observable, switchMap } from 'rxjs';
import { PoList } from '@core/models/booking/po.model';
import { BookingDetail } from '@core/models/booking/booking-header.model';


@Component({  
  templateUrl: 'po-dialog.component.html',
  encapsulation: ViewEncapsulation.None
})

export class PoDialogComponent implements OnInit {
  warehouseCode: string;
  supplierCode: string;
  supplierName: string;
  bookingDate: Date;
  internalSupGroupId: number;
  // poList: any[];

  bookingDtl: PoList[];
  pos: PoList[];
  selectedPos: PoList;
  
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  )
  {
    this.bookingDtl = config.data.poList;
  }

  ngOnInit()
  {
    this.fetchData();
  }

  fetchData() : void
  {
    this.pos = this.bookingDtl;
  }

  confirm(){
    this.ref.close(this.selectedPos);
  }

}
