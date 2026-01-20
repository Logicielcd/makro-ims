import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';
import { PoService } from '@core/services/booking/po.service';

import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { concatMap, map, Observable, switchMap } from 'rxjs';
import { PoList } from '@core/models/booking/po.model';
import { FilterMatchMode, MessageService, SelectItem } from 'primeng/api';

@UntilDestroy()

@Component({  
  templateUrl: 'po-dialog.component.html',
  encapsulation: ViewEncapsulation.None
})

export class PoDialogComponent implements OnInit {

  msgs: any[];
  matchModeOptions: SelectItem[];
  
  warehouseCode: string;
  supplierCode: string;
  supplierName: string;
  bookingDate: Date;
  internalSupGroupId: number;
  selectedPoList: string[];

  pos: PoList[];
  selectedPos: PoList;

  cols: any[];
  
  constructor(
    public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private authService: AuthService    
    ,private poService: PoService
    ,private messageService: MessageService
    )
  {
    this.warehouseCode = config.data.warehouseCode;
    this.bookingDate = config.data.bookingDate;
    this.supplierCode = config.data.supplierCode;
    this.supplierName = config.data.supplierName;
    this.internalSupGroupId = config.data.internalSupGroupId;
    this.selectedPoList = config.data.selectedPoList;
  }

  ngOnInit()
  {
    this.cols = [
      { field: 'warehouse_Code', header: 'Warehouse' },
      { field: 'warehouse_Name', header: 'Warehouse Name' },
      { field: 'company_Code', header: 'Company' },
      { field: 'company_Name', header: 'Company Name' },
      { field: 'po_Nbr', header: 'PO No.' },     
      { field: 'merch_Type', header: 'Operation Type' },      
      { field: 'plan_Receive_Date', header: 'Delivery Date' },      
      { field: 'sup_Code', header: 'Sup Code'},
      { field: 'weight', header: 'Weight'},
      { field: 'booking_Id', header: 'Is Booked'},
    ];

    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS }
    ];
    
    this.fetchData();
  }

  fetchData() : void
  {
    this.poService.getBySupCode(this.supplierCode,this.bookingDate)
    .pipe(untilDestroyed(this))
    .subscribe({next:(data) => {
      this.pos = data;      
      this.selectedPoList.forEach(po => {
        let index = this.pos.findIndex(x=>x.po_Nbr == po);
        this.pos.splice(index,1);
      });
    },
    error:(error)=>{      
      this.msgs = [];
      error.Messages.forEach((msg: any) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
      });
    }});
  }

  confirm(){
    this.ref.close(this.selectedPos);
  }

}
