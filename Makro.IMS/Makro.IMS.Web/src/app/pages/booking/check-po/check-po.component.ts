import { Component, ElementRef, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { NgForm } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';
import { PoService } from '@core/services/booking/po.service';
import { PoComment, PoDcDelay, PoList, PoLog } from '@core/models/booking/po.model';
import { MessageService } from 'primeng/api';
import { catchError, switchMap } from 'rxjs';

@UntilDestroy()

@Component({  
  templateUrl: 'check-po.component.html',
//   styleUrls: ['check-po.component.scss'],
  encapsulation: ViewEncapsulation.None,
})

export class CheckPoComponent implements OnInit {
  user : User;
  visible: boolean = false;
  poNbr : string;
  msgs: any;
  poList: PoList[];
  cols: any[];
  poComment: PoComment = {} as PoComment;
  openDialog: boolean = false;
  poLog: PoLog[];

  isComment: boolean;
  isDcDelay: boolean;
  isCommentView: boolean;
  isDcDelayView: boolean;
  isPoLog: boolean;

  @ViewChild('f') f: NgForm;

  constructor(private authService: AuthService,
    private poService: PoService,
    private messageService: MessageService
    ) {      
      this.user = this.authService.getUser();
  }

  ngOnInit() {
    this.cols = [
        {
          field: 'warehouse_Code',
          display: 'string',
          header: 'Warehouse'
        },
        {
          field: 'company_Code',
          display: 'string',
          header: 'Company'
        },
        {
          field: 'merch_Type',
          display: 'string',
          header: 'Operation Type',
        },
        {
            field: 'sup_Code',
            display: 'string',
            header: 'Sup Code',
        },
        {
          field: 'po_Nbr',
          display: 'string',
          header: 'PoNo',
        },
        {
          field: 'create_Date',
          display: 'datetime',
          header: 'PO Create Date',
        },
        {
          field: 'plan_Receive_Date',
          display: 'date',
          header: 'Plan Delivery Date',
        },
        {
          field: 'expire_Date',
          display: 'date',
          header: 'Expire Date',
        },
        {
            field: 'remark',
            display: 'string',
            header: 'Booking Id',
        },
        {
          field: 'total_Qty',
          display: 'string',
          header: 'TotalQty',
        },
        {
          field: 'full',
          display: 'string',
          header: 'Full',
        },
        {
          field: 'half',
          display: 'string',
          header: 'Half',
        },
        {
          field: 'con',
          display: 'string',
          header: 'loose',
        },        
        {
          field: 'weight',
          display: 'string',
          header:'KG/CS',
        },
        {
          field: 'is_Delay',
          display: 'bool',
          header:'Late',
        },
        {
          field: 'delay_Reason',
          display: 'string',
          header:'Delay Reason',
        }
      ];

    this.fetchData();
  }

  fetchData() : void{

    if(this.user.userType != 'SUP' && this.user.userType != 'SUPTRAN' && this.user.userType != 'BH'){
      this.isCommentView = true;
      this.isDcDelayView = true;
      this.isComment = false;
      this.isDcDelay = false;
      this.isPoLog = false;
      if(this.user.userType === 'ADMIN' || this.user.userType === 'CONTROL'){
        this.isComment = true;
      }
      if(this.user.userType === 'ADMIN' || this.user.userType === 'CONTROL' || this.user.userType === 'WAIVE'){
        this.isDcDelay = true;        
      }
    }
    else{
      this.isComment = false;
      this.isDcDelay = false;
    }

    this.openDialog = false;
    this.poNbr = '';
  }

  refresh() : void{
    window.location.reload();
  }

  onCheckPo(){
    this.poService.getPoByPo(this.poNbr)
    .pipe(
      untilDestroyed(this), // จัดการ lifecycle
      switchMap((data) => {
        // เก็บข้อมูลจาก API ตัวแรก
        this.poList = data;
        this.poList.forEach(po => {
          po.con = po.con + po.non;

          if(po.delay_Reason == 'DC Delay'){
            po.isDelayReason = true;
          }
          else{
            po.isDelayReason = false;
          }
        });

        // เรียก API ตัวที่สอง
        return this.poService.getComment(this.poNbr);
      }),
      catchError((error) => {
        // จัดการ error และคืนค่า observable ที่ไม่ล้มเหลว
        this.poList = null;
        this.msgs = [];
        error.Messages.forEach((msg: any) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: msg,
            life: 3000,
          });
        });
        return []; // คืนค่า observable เปล่า
      })
    )
    .subscribe((poComment) => {
      if(poComment === null){
        this.poComment = {} as PoComment;
        this.poComment.po = this.poNbr;
        this.poComment.comment1 = '';
        this.poComment.comment2 = '';
        this.poComment.comment3 = '';
        this.poComment.comment4 = '';
        this.poComment.comment5 = '';
        this.poComment.comment6 = '';
      }else{
        this.poComment = poComment;
      } // จัดการข้อมูลจาก API ตัวที่สอง

    });
  }

  onSaveComment(){    
    this.poService.addComment(this.poComment)
    .pipe(untilDestroyed(this))
    .subscribe({next:(data)=>{
      this.messageService.add({severity:'success', summary: 'Update comment completed', detail: ""});     
    },
    error(error) {
      this.poList = [];
      this.msgs = [];
            error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
    }})
  }

  onSaveDcDelay(){    
    let poDcDelay: PoDcDelay = {} as PoDcDelay;
    poDcDelay.poNo = this.poList[0].po_Nbr;
    poDcDelay.delayReason = this.poList[0].isDelayReason ? 'DC Delay':'Not Receive On-Time';

    this.poService.updateDcDelay(poDcDelay)
    .pipe(untilDestroyed(this))
    .subscribe({next:(data)=>{
      this.messageService.add({severity:'success', summary: 'Update Delay Reason completed', detail: ""});     
    },
    error(error) {
      this.poList = [];
      this.msgs = [];
            error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
            });
    }})
  }

  openLog(){
    this.openDialog = true;

    this.poService.getPoLog(this.poNbr)
    .pipe(untilDestroyed(this))
    .subscribe({next: (data) =>{
      this.poLog = data;
      this.poLog.forEach(log => {
        log.dateTimeStamp = new Date(log.dateTimeStamp);
      });
    }})

  }
}
