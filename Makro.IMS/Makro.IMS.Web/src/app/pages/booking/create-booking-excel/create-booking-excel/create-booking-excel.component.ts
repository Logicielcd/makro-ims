import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { ConfirmationService, MenuItem, Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';

/* service */
import { SupplierService } from '@core/services/master/supplier.service';
import { BookingHeaderService } from '@core/services/booking/booking-header.service';
import { CreateBookingExcelService } from '@core/services/booking/create-booking-excel.service';
import { WarehouseService } from '@core/services/master/warehouse.service';
import { TruckMasterService } from '@core/services/master/truckMaster.service';

/* model */
import { User } from '@core/models/account/user.model';
import { Supplier } from '@core/models/master/supplier.model';
import { WarehouseCapacity } from '@core/models/master/warehouse-capacity.model';
import { Warehouse } from '@core/models/master/warehouse.model';
import { CreateBookingExcelWarehouse,CreateBookingExcelDto } from '@core/models/booking/booking-create-excel.model';
import { BookingHeader } from '@core/models/booking/booking-header.model';

import { environment } from '@environments/environment';

import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { TruckMaster } from '@core/models/master/truck-master.model';
import { RowGroupHeader } from 'primeng/table';


type AOA = any[][];

@UntilDestroy()
@Component({
  templateUrl: './create-booking-excel.component.html',
  styleUrls: ['create-booking-excel.component.scss'],
})

export class CreateBookingExcelComponent implements OnInit {

  title = 'Create Booking by excel import';

  user: User;

  items: MenuItem[];
  msgs: Message[] = [];

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  isSelect: boolean = false;

  ref: DynamicDialogRef;

  // auth
  supReadonly: boolean = false;

  // warehouse variable
  warehouse: Warehouse[];
  warehouseCapacity: WarehouseCapacity[];

  // supplier variable
  suppliers: Supplier[];
  supplierSelected: string;

  // truck type
  trucks: TruckMaster[];

  // booking data variable
  bookingDate: Date;
  minDate: Date;

  contactName:string;
  contactEmail:string;
  contactPhone:string;

  isCompleted: boolean = false;
  isUploaded: boolean = false;

  // table column variable  
  excelCols: any[];
  resultCols: any[];

  importExcelData: AOA;
  //importWhseData: AOA;
  dataSourceExcel: CreateBookingExcelWarehouse[];
  dataSourceResult: BookingHeader[];

  dataBookingHeader: BookingHeader[];
  
  createBookingExcel: CreateBookingExcelDto;

  isTimeSlotChange: boolean;

  fileName: string;

  wopts: XLSX.WritingOptions = { bookType: 'xlsx', type: 'array' };

  @ViewChild('f') f: NgForm;

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    private authService: AuthService,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private bookingHeaderService: BookingHeaderService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private supplierService: SupplierService,    
    private createBookingExcelService: CreateBookingExcelService,
    private warehouseService: WarehouseService,
    private truckMasterService: TruckMasterService,
  ) 
  {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

    this.bookingDate = new Date();
    this.bookingDate.setDate(this.bookingDate.getDate() + 1);

    this.minDate = new Date();
    this.minDate.setDate(this.minDate.getDate() + 1);

    this.user = this.authService.getUser();
  }

  ngOnInit(){
    
    this.fileName = "";
    this.msgs = [];    
    this.excelCols = [];
    this.resultCols = [];

    this.isCompleted = false;
    this.isUploaded = false;
    this.isSelect = false;
    this.isTimeSlotChange = false;

    this.excelCols = [
     {
        field: 'warehouseCode',
        display: 'string',
        header: 'Warehouse',
     },
     {
      field: 'poNo',
      display: 'string',
      header: 'PoNo',
     },
     {
        field: 'truckType',
        display: 'string',
        header: 'Truck Type',
     },
     {
      field: 'deliveryType',
      display: 'string',
      header: 'Backhaul',
      },
     {
        field: 'truckNo',
        display: 'string',
        header: 'Truck No',
     },
     {
      field: 'truckGroup',
      display: 'string',
      header: '1 Booking ID Multi Truck',
     },     
     {
        field: 'timeSlot',
        display: 'time',
        header: 'Time Slot',
      },
      {
        field: 'remark',
        display: 'string',
        header: 'Remark',
      },
      {
        field: 'bookingGroup',
        display: 'string',
        header: 'Booking Group',
      },
      {
        field: 'inputDataValidate',
        display: 'string',
        header: 'Excel file validation Result',
      },
      {
        field: 'importResult',
        display: 'string',
        header: 'Import Result',
      },
      {
        field: 'systemOverride',
        display: '',
        header: ''
      },
    ];

    this.resultCols = [
      {
         field: 'warehouseCode',
         display: 'string',
         header: 'Warehouse',
      },
      {
        field: 'bookingId',
        display: 'string',
        header: 'Booking Id',
      },    
      {
        field: 'merchType',
        display: 'string',
        header: 'Operation Type',
      },
      {
         field: 'bookingStart',
         display: 'datetime',
         header: 'Start Time',
      },
      {
         field: 'bookingEnd',
         display: 'datetime',
         header: 'End Time',
      },
      {
        field: 'revisionPrefix',
        display: 'string',
        header: 'Booking Group',
     },
     {
      field: 'status',
      display: 'string',
      header: 'Status',
   },
    ];

    this.fetchData();
  }

  initialScreen(){
    this.dataSourceExcel = {} as CreateBookingExcelWarehouse[];  
    this.dataSourceResult = {} as BookingHeader[];
    this.dataBookingHeader = {} as BookingHeader[];
    this.msgs = [];
    this.isCompleted = false;
    this.isSelect = false;
    this.isTimeSlotChange = false;

    this.fetchData();
  }

  onCreateTrip() {
    this.confirmDialog = true;
  }

  export(){    
    let link = document.createElement('a');
    link.setAttribute('type', 'hidden');
    link.href = `${environment.webUrl}` + '/assets/files/createbooking_template.xlsx';
    link.download = 'createbooking_template.xlsx';
    document.body.appendChild(link);
    link.click();
    link.remove();    
  }

  importfile(event){       
    this.dataSourceExcel = [];
    this.dataSourceResult = {} as BookingHeader[];
    this.dataSourceResult = [];

    this.dataBookingHeader = {} as BookingHeader[];
    
    const target: DataTransfer = <DataTransfer>event.target;
    
    if (target.files.length !== 1) throw new Error('Cannot use multiple files');
    
    const reader: FileReader = new FileReader();
    // reader.readAsBinaryString(event.target.files[0]);    
    reader.onload = (e: any) => {
      const bstr: string = e.target.result;
      const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', raw: true, cellText:true, cellDates:true });
      let wsname: string = wb.SheetNames[0];
      let ws: XLSX.WorkSheet = wb.Sheets[wsname];

      this.importExcelData = <AOA>(
        XLSX.utils.sheet_to_json(ws, { blankrows: false, header: 1 })
      );
      this.parseExcelData(this.importExcelData);          
    };
    
    reader.readAsBinaryString(target.files[0]);
  
    this.fileName="";
  }

  parseExcelData(rawData: AOA) {
    this.msgs = [];
    const rawOrders = rawData.map(
      (cols) =>          
        (
          {
            warehouseCode: cols[0]?.toString(),
            poNo: cols[1]?.toString(),
            truckType: cols[2]?.toString(),
            deliveryType: cols[3]?.toString(),
            truckNo: cols[4]?.toString(),
            truckGroup: cols[5]?.toString(),
            timeSlot: cols[6]?.toString().length > 0 ? new Date(2022,1,1, new Date(cols[6]?.toString()).getHours(), new Date(cols[6]?.toString()).getMinutes()) : null,
            remark: cols[7]?.toString(),            
            importResult: "",
            inputDataValidate: "",
            bookingGroup: "",            
        } as CreateBookingExcelWarehouse
        )
    );
    rawOrders.shift();
    rawOrders.shift();

    this.dataSourceExcel = rawOrders;    
    
    this.isCompleted = true;
    this.dataSourceExcel.forEach(excel => {
        let validationErrors = [];
        if (!this.trucks.some(x => x.truckCode === excel.truckType)) {
            this.isCompleted = false;
            validationErrors.push('ไม่พบข้อมูล หรือ ข้อมูล Truck Type ไม่ถูกต้อง');
        }
        if (!excel.truckNo) {
            this.isCompleted = false;
            validationErrors.push('ไม่พบข้อมูล Truck No');
        }
        if (!excel.timeSlot) {
            this.isCompleted = false;
            validationErrors.push('ไม่พบข้อมูล Time slot');
        }

        if(this.user.userType == 'SUP'){
          if(excel.deliveryType == 'Backhaul'){
            this.isCompleted = false;
            validationErrors.push('ไม่สามารถเลือกรถที่จัดส่งเป็นรถ Backhaul');
          }
        }

        excel.inputDataValidate = validationErrors.join('\r\n');
    });

    const groupedData = this.dataSourceExcel.reduce((acc, item) => {
      // Create a unique key based on warehouse and company
      item.truckGroup = item.truckGroup == undefined ? '' : item.truckGroup;
      const key = `${item.warehouseCode}-${item.truckGroup}-${item.timeSlot}}`;
      // If the key doesn't exist in accumulator, add it with an empty array
      if (!acc[key]) {
        acc[key] = { warehouse: item.warehouseCode, truckGroup: item.warehouseCode + '-' + item.truckGroup, pos: [] };
      }
      // Push the truck info into the trucks array for that warehouse-company combo
      acc[key].pos.push(item.poNo);
      return acc;
    }, {} as Record<string, { warehouse: string; truckGroup: string; pos: string[] }>);
    
    // Convert the object back to an array if you prefer array output
    const result = Object.values(groupedData);

    result.forEach((res)=>{
      this.dataSourceExcel.forEach((ex)=>{
        if(res.pos.indexOf(ex.poNo) >= 0)
          ex.bookingGroup = res.truckGroup;        
      });
    });
  }

  fetchData() 
  {    
    this.warehouseService.getAll().pipe(untilDestroyed(this)).subscribe((data)=>{
      this.warehouse = data;
      if(this.user.userType.toUpperCase() != "SUP"){
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
      this.truckMasterService.getAll().pipe(untilDestroyed(this)).subscribe((data)=>{
        this.trucks = data;
      });
    });    
  }

  onSelectSup(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);

    this.bookingHeaderService
    .getByBookingHeaderBySupCodeAndBookingDate(sup.supCode,this.bookingDate)
    .pipe(untilDestroyed(this))
    .subscribe(
      {next: (data) =>
        {
          this.dataBookingHeader = data;
          if(this.contactName.length > 0 && this.contactEmail.length > 0 && this.contactPhone.length > 0){
            this.isSelect = true;
          }
      
        },
        error: (error) =>
        {          
          this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
          });
        }
      }
    );

  }

  getSupplierValue(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);
    if(sup != null && sup != undefined){
      this.contactName = sup.contactName;
      this.contactEmail = sup.contactEMail;
      this.contactPhone = sup.phoneNumber;
    }
  }

  importBooking(){    
    if(this.isTimeSlotChange){
      this.confirmationService.confirm({
        message: 'พบ warehouse ที่มีเวลา booking ไม่ตรงกับข้อมูล booking ก่อนหน้า ต้องการนำเข้าข้อมูลหรือไม่',
        header: 'Confirm',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.importBookingData();
        }
      });
    }else{
      this.importBookingData();
    }    
  }

  importBookingData(){
    let sup = this.suppliers.find(x=>x.supCode == this.supplierSelected);

    this.createBookingExcel = {} as CreateBookingExcelDto;    
    this.createBookingExcel.createBookingExcelWarehouses = [] as CreateBookingExcelWarehouse[];
    this.createBookingExcel.createBookingExcelWarehouses = this.dataSourceExcel;
    this.createBookingExcel.bookingHeaders = [] as BookingHeader[];
    this.createBookingExcel.supplierCode = this.supplierSelected;
    this.createBookingExcel.contactName = this.contactName;
    this.createBookingExcel.contactEmail = this.contactEmail;
    this.createBookingExcel.contactPhone = this.contactPhone;
    this.createBookingExcel.internalSupGroupId = sup.internalGroupId;
    this.createBookingExcel.bookingDate = this.bookingDate;

    this.isCompleted = false;

    this.createBookingExcelService
    .importExcel(this.createBookingExcel)
    .pipe(untilDestroyed(this))
    .subscribe(
      {next: (data) =>
        {          
          this.dataSourceExcel = data.createBookingExcelWarehouses;
          this.dataSourceResult = data.bookingHeaders;
          this.isUploaded = true;
        },
        error: (error) =>
        {
          this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.messageService.add({severity:'error', summary: 'Error', detail: msg, life: 3000});
          });
        }
      }
    );

  }

  refresh(): void {
    this.initialScreen();
  }

  exportResult(){    
    var wb = XLSX.utils.book_new();    

    var wsWhseData = [
      [ "Warehouse","PO No","Truck Type","Truck No.","1 Booking ID Multi Truck","Appt. time(HH:mm)","Remark","Import Result" ]
    ];
    var wsWhseColWidth = [{wch:15},{wch:15},{wch:15},{wch:15},{wch:20},{wch:15},{wch:20},{wch:20}];

    this.dataSourceExcel.forEach(whse => {
      whse.timeSlot = new Date(whse.timeSlot);

      wsWhseData.push([whse.warehouseCode,whse.poNo,whse.truckType,whse.truckNo.toString(),whse.truckGroup
        ,whse.timeSlot != null ? whse.timeSlot.getHours().toString().padStart(2,'0') + ':' + whse.timeSlot.getMinutes().toString().padStart(2,'0') : ""
        ,whse.remark,whse.importResult]);
    });

    var wsWhse = XLSX.utils.aoa_to_sheet(wsWhseData);
    
    wsWhse['!cols'] = wsWhseColWidth;
    
    var wsResultData = [
      [ "Warehouse","Booking Id","Start Time","End Time" ]
    ];    
    var wsResultColWidth = [{wch:15},{wch:15},{wch:20}];

    this.dataSourceResult.forEach(res => {

      res.bookingStart = new Date(res.bookingStart);
      res.bookingEnd = new Date(res.bookingEnd);
      wsResultData.push([res.warehouseCode,res.bookingId,
        res.bookingStart != null ? res.bookingStart.getDate().toString().padStart(2,'0') + '/' + res.bookingStart.getMonth().toString().padStart(2,'0') + '/' 
        + res.bookingStart.getFullYear().toString() + ' ' + res.bookingStart.getHours().toString().padStart(2,'0') + ':' + res.bookingEnd.getMinutes().toString().padStart(2,'0') : "",
        res.bookingEnd != null ? res.bookingEnd.getDate().toString() + '/' + res.bookingEnd.getMonth().toString() + '/' 
        + res.bookingEnd.getFullYear().toString() + ' ' + res.bookingEnd.getHours().toString().padStart(2,'0') + ':' + res.bookingEnd.getMinutes().toString().padStart(2,'0') : ""]
      );
    });

    var wsResult = XLSX.utils.aoa_to_sheet(wsResultData);
    wsResult['!cols'] = wsResultColWidth;
    
    XLSX.utils.book_append_sheet(wb,wsWhse,"Booking");
    XLSX.utils.book_append_sheet(wb,wsResult,"Summary Result");

    const excelBuffer: any = XLSX.write(wb, {
      bookType: 'xlsx',
      type: 'array',
    });

    const EXCEL_TYPE =
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([excelBuffer], {
      type: EXCEL_TYPE,
    });
    
    FileSaver.saveAs(
      data,
      'importresult_' + new Date().getTime() + EXCEL_EXTENSION
    );

  }

}
