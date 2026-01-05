import { Component, NgModule, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AuthService } from 'auth/auth.service';
import { PoService } from '@core/services/booking/po.service';

import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { concatMap, map, Observable, switchMap } from 'rxjs';
import { PoList } from '@core/models/booking/po.model';
import { BookingDetail } from '@core/models/booking/booking-header.model';
import { FilterMatchMode, SelectItem } from 'primeng/api';


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
  poList: any[];

  pos: BookingDetail[] = [];
  selectedPos: PoList;

  selectedPoList: string[];
  
  cols: any[];
  
  matchModeOptions: SelectItem[];
  
  
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  )
  {
    this.poList = config.data.poList;
    this.selectedPoList = config.data.selectedPoList;
  }

  ngOnInit()
  {
    this.cols = [      
      { field: 'poNbr', header: 'PO No.' },      
    ];

    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS }
    ];

    this.fetchData();
  }

  fetchData() : void
  {
    this.pos = this.poList;

    this.selectedPoList.forEach(po => {
      let index = this.pos.findIndex(x=>x.poNbr == po);
      this.pos.splice(index,1);
    });

  }

  confirm(){
    this.ref.close(this.selectedPos);
  }

}
