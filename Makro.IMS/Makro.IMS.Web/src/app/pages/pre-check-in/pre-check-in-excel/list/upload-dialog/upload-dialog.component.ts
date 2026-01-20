import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import { User } from '@core/models/account/user.model';
import { AuthService } from 'auth/auth.service';

import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { concatMap, map, Observable, switchMap } from 'rxjs';
import { Supplier } from '@core/models/master/supplier.model';
import { SupplierService } from '@core/services/master/supplier.service';
import { LanguageService } from '@core/services/language.service';
import { TranslateService } from '@ngx-translate/core';
import * as XLSX from 'xlsx';
import { PreCheckInExcelDetailDto, PreCheckInExcelDto } from '@core/models/booking/pre-check-in-excel.model';
import { PreCheckInExcelService } from '@core/services/booking/pre-check-in-excel.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';
import { TruckMaster } from '@core/models/master/truck-master.model';

type AOA = any[][];

@UntilDestroy()

@Component({  
  templateUrl: 'upload-dialog.component.html',
  encapsulation: ViewEncapsulation.None
})

export class UploadDialogComponent implements OnInit { 
  
  title = 'Create Booking';
  user: User;
  
  cols: any[] = [];

  // supplier variable
  suppliers: Supplier[] = [];
  supplierSelected: string;

  isSelect: boolean = false;
  isCompleted: boolean = false;
  isLoading: boolean = false;
  warehouseCode: string;
  supplierCode: string;
  supplierName: string;
  bookingDate: Date;
  internalSupGroupId: number;
  fileName: string;
  
  truckMasters: TruckMaster[] = [];

  importPreCheckInData: AOA;
  dataSourcePreCheckIn: PreCheckInExcelDetailDto[] = [];

  data: PreCheckInExcelDetailDto[];
  preCheckInDto: PreCheckInExcelDto;

  constructor(
    public ref: DynamicDialogRef
    ,public config: DynamicDialogConfig
    ,private supplierService: SupplierService
    ,private authService: AuthService    
    ,private languageService: LanguageService
    ,private translateService: TranslateService
    ,private preCheckInExcelService: PreCheckInExcelService
    ,private truckSevice: TruckMasterService
    )
  {

    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

    this.bookingDate = new Date();
    this.bookingDate.setDate(this.bookingDate.getDate() + 1);
    this.user = this.authService.getUser();
    
  }

  ngOnInit()
  {
    this.fileName = "";
    this.isSelect = false;
    this.isCompleted= false;
    this.isLoading = false;
    this.dataSourcePreCheckIn = [] as PreCheckInExcelDetailDto[];

    this.cols = [
      {
         field: 'bookingId',
         display: 'string',
         header: 'Booking Id',
      },
      {
        field: 'warehouseCode',
        display: 'string',
        header: 'Warehouse',
      },
      {
        field: 'truckNo',
        display: 'string',
        header: 'Truck No.',
      },
      {
         field: 'truckType',
         display: 'string',
         header: 'Truck Type',
      },
      {
         field: 'truckLicense',
         display: 'string',
         header: 'Truck License Head',
      },
      {
        field: 'truckLicense2',
        display: 'string',
        header: 'Truck License Trail',
     },
      {
         field: 'driverName',
         display: 'string',
         header: 'Driver Name',
       },
       {
        field: 'telNo',
        display: 'string',
        header: 'Tel. No',
       },
       {
        field: 'lineNo',
        display: 'string',
        header: 'Line Account',
      },
      {
        field: 'validateExcel',
        display: 'string',
        header:'Validate Excel',
      },
      {
        field: 'importResult',
        display: 'string',
        header: 'Import Result',
      },
     ];
 
    this.fetchData();
  }

  fetchData() : void
  {
    
    this.truckSevice.getAll()
    .pipe(untilDestroyed(this))
    .subscribe((data)=> this.truckMasters = data);

    if(this.user.userType.toUpperCase() !== "SUP"){
      this.supplierService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.suppliers = data;

      this.getSupplierValue();
      });
    }
    else{

    this.supplierService
      .getBySupGroup(this.user.internalSupGroupId)
      .pipe(untilDestroyed(this))
      .subscribe((data) => {
        this.suppliers = data;

      this.getSupplierValue();
      });
    }

  }

  confirm(){
    this.ref.close();
  }

  getSupplierValue(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);
    if(sup != null && sup != undefined){
      this.supplierCode = sup.supCode;
      this.supplierName = sup.supName;
      this.internalSupGroupId = sup.internalSupId;
      this.isSelect = true;
    }
  }

  importfile(event){
    this.isLoading = true;
    this.dataSourcePreCheckIn = [] as PreCheckInExcelDetailDto[];

    const target: DataTransfer = <DataTransfer>event.target;
    
    if (target.files.length !== 1) throw new Error('Cannot use multiple files');
    
    const reader: FileReader = new FileReader();
    // reader.readAsBinaryString(event.target.files[0]);    
    reader.onload = (e: any) => {

      const bstr: string = e.target.result;
      const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', raw: true, cellText:true, cellDates:true });
      let wsname: string = wb.SheetNames[0];
      let ws: XLSX.WorkSheet = wb.Sheets[wsname];

      this.importPreCheckInData = <AOA>(
        XLSX.utils.sheet_to_json(ws, { blankrows: false, header: 1 })
      );

      this.parsePreCheckInData(this.importPreCheckInData);
      
    };
    
    reader.readAsBinaryString(target.files[0]);
  
    this.fileName="";
    this.isLoading = false;
  }

  parsePreCheckInData(rawData: AOA) {
    const rawOrders = rawData.map(
      (cols) =>
        (
          {
            bookingId: cols[0]?.toString(),
            warehouseCode: cols[1]?.toString(),
            truckNo: cols[4]?.toString(),
            truckType: cols[5]?.toString(),
            truckLicense: cols[6]?.toString(),
            truckLicense2: cols[7]?.toString(),
            driverName: cols[8]?.toString(),
            telNo: cols[9]?.toString(),
            lineNo: cols[10]?.toString(),
            supCode: this.supplierCode,
            internalSupGroupId: this.internalSupGroupId,
            importResult: "",
            validateExcel: "",
        } as PreCheckInExcelDetailDto)
    );
    rawOrders.shift();
    this.dataSourcePreCheckIn = rawOrders;

    this.isCompleted = true;

    this.dataSourcePreCheckIn.forEach(preCheckIn => {
      if(preCheckIn.bookingId == null || preCheckIn.truckType == null || preCheckIn.truckLicense == null || preCheckIn.driverName == null
         || preCheckIn.telNo == null || preCheckIn.lineNo == null) {
        preCheckIn.validateExcel = "Please check the input value on excel file , กรุณาตรวจสอบข้อมูลในไฟล์ excel อีกครั้ง";
        this.isCompleted = false;
      }
    });

    const telpattern = /^0\d{8,9}$/;
    const pattern = /^(?:\d{1}[ก-ฮ]{1,2}|\d{3}|\d{2}|[ก-ฮ]{1,2}?)[-]{1}\d{1,4}$/;     
    let licenseList : string[] = [];
    // validate data
    if(this.isCompleted == true){
      this.dataSourcePreCheckIn.forEach(preCheckIn=>{

        const truckType = this.truckMasters.find(x=>x.truckCode == preCheckIn.truckType);
        if(truckType === null){
          this.isCompleted = false;
          preCheckIn.validateExcel += "ประเภทรถไม่ถูกต้อง" + "\r\n";
        }
        else{
          preCheckIn.truckSequence = truckType.sequence;
        }

        if(preCheckIn.truckLicense !== null && preCheckIn.truckLicense !== undefined && preCheckIn.truckLicense.length > 0){
          const matches = preCheckIn.truckLicense.match(pattern);
          if(matches === null){
            this.isCompleted = false;
            preCheckIn.validateExcel += "รูปแบบทะเบียนรถไม่ถูกต้อง" + "\r\n";
          }
          else{
            if(matches[0] !== preCheckIn.truckLicense){
              this.isCompleted = false;
              preCheckIn.validateExcel += "รูปแบบทะเบียนรถไม่ถูกต้อง" + "\r\n";
            }
          }
        }
        if(preCheckIn.truckLicense2 !== null && preCheckIn.truckLicense2 !== undefined && preCheckIn.truckLicense2.length > 0){
          const matches = preCheckIn.truckLicense2.match(pattern);
          if(matches === null){
            this.isCompleted = false;
            preCheckIn.validateExcel += "รูปแบบทะเบียนรถไม่ถูกต้อง" + "\r\n";
          }
          else{
            if(matches[0] !== preCheckIn.truckLicense2){
              this.isCompleted = false;
              preCheckIn.validateExcel += "รูปแบบทะเบียนรถไม่ถูกต้อง"+ "\r\n";
            }
          }
        }
        if(preCheckIn.telNo !== null && preCheckIn.telNo !== undefined && preCheckIn.telNo.length > 0){
          const matches = preCheckIn.telNo.match(telpattern);
          if(matches === null){
            this.isCompleted = false;
            preCheckIn.validateExcel += "รูปแบบหมายเลขโทรศัพท์ไม่ถูกต้อง"+ "\r\n";
          }
          else{
            if(matches[0] !== preCheckIn.telNo){
              this.isCompleted = false;
              preCheckIn.validateExcel += "รูปแบบหมายเลขโทรศัพท์ไม่ถูกต้อง"+ "\r\n";
            }
          }
        }

        licenseList.push(preCheckIn.truckLicense + '|' + preCheckIn.truckLicense2);
      });

      // check duplicate license plate
      const dup = licenseList.filter((item,index) => licenseList.indexOf(item) !== index);
      
      if(dup.length > 0){
        this.isCompleted = false;
        dup.forEach(d => {
          const matchingItems = this.dataSourcePreCheckIn.filter(
            (x) => `${x.truckLicense}|${x.truckLicense2}` === d
          );
      
          matchingItems.forEach((item) => {
            item.validateExcel = (item.validateExcel || "") + "ทะเบียนรถซ้ำ\r\n";
          });
        });
      }
    }

  }

  importPreCheckIn(){
    this.isLoading = true;
    this.preCheckInDto = {}as PreCheckInExcelDto;
    this.preCheckInDto.preCheckInExcelDetailDtos = [] as PreCheckInExcelDetailDto[];
    this.preCheckInDto.preCheckInExcelDetailDtos = this.dataSourcePreCheckIn;

    this.preCheckInExcelService
    .importExcel(this.preCheckInDto)
    .pipe(untilDestroyed(this))
    .subscribe(
      {next: (data) =>
        {
          this.preCheckInDto = data;          
          this.dataSourcePreCheckIn = this.preCheckInDto.preCheckInExcelDetailDtos;
          this.isLoading = false;
        },
        error: (error) =>
        {
          
          // this.msgs = [];
          // error.Messages.forEach((msg: any) => {
          //   this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
          // });
          this.isLoading = false;
        }
      }
    );
  }

  refresh(): void {
    this.ngOnInit();
  }

}
