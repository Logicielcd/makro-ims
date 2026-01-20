import { OnInit, Component, ElementRef, NgModule, ViewChild, ViewEncapsulation, OnDestroy } from '@angular/core';
import { AuthService } from 'auth/auth.service';

import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { ReportService } from '@core/services/report/report.service';

import html2pdf from 'html2pdf.js';
import { APP_MENU } from '@core/models/menu/app-menu.model';

@Component({  
  templateUrl: 'report-dialog.component.html',
  styleUrls: ['report-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})


export class ReportDialogComponent implements OnInit,OnDestroy {

  rowsPerPage: number;
  barcodeData: string;
  internalHeaderKey: number;
  dataSource: any[];
  canModify: boolean;
  truckLists: any[] = [];
  printDate: Date;

  @ViewChild('content', { static: false }) content: ElementRef;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private reportService: ReportService,
    private authService: AuthService,
  )
  {
    this.authService
    .canModify(APP_MENU.precheckin.module, APP_MENU.precheckin.preCheckInCompleted)
    .subscribe((granted) => (this.canModify = granted));

    this.internalHeaderKey = config.data.bookingId;
  }
  
  ngOnInit()
  {    

    this.printDate = new Date();
    this.rowsPerPage = 8;

    this.reportService.getGatePass(this.internalHeaderKey)    
    .subscribe((data)=>{
      this.dataSource = data;
      this.dataSource.forEach((d) =>{
        d.reasonDisplay = d.reason === 'DC Delay' ? '(ไม่ปรับ)' : d.reason === 'Not Receive On-Time' ? '(ปรับ)' : '';
        d.backHaulDisplay = d.backHaul ? 'BH' : 'Direct';
      });

      let trucks = this.dataSource.map(item => item.id)
      .filter((value, index, self) => self.indexOf(value) === index);

      trucks.forEach(truck => {
        let truckInfo = this.dataSource.filter((v)=> v.id == truck);

        let pages = Math.ceil(truckInfo.length/this.rowsPerPage);
        let pageList: any[] = [];

        let pageSummary = truckInfo.reduce((a,b) => {
          return {
              totalQty: a.totalQty + b.totalQty,
              fullPl : a.fullPl + b.fullPl,
              halfPl : a.halfPl + b.halfPl,
              looseQty: a.looseQty + b.looseQty
            }
          },{ totalQty:0, fullPl:0,halfPl:0,looseQty:0});

        for (let index = 0; index < pages; index++) {   
          let poList: any[] = [];
          let row = 0;
          let toRow = 0;

          row = index * this.rowsPerPage;
          toRow = (index + 1) * this.rowsPerPage;
          
         // console.log('from : ' + row + ' to : ' + toRow);

          if(index > 0){
            //toRow = toRow + 2;
          }

          if(toRow > truckInfo.length){
            toRow = truckInfo.length;
          }

          for (row; row < toRow; row++) {       
            truckInfo[row].no = row + 1;     
            poList.push(truckInfo[row]);
          }
          
          pageList.push({pageNo: index+1, pageInfo: poList});
        }

        this.truckLists.push({licensePlate: truckInfo[0].licensePlate,pageInfo: pageList,pageGroup: pageSummary});

      });
      
    });
    
  }
  
  ngOnDestroy() {
    if (this.ref) {
      this.ref.close();
    }
  }

  printReport(){
    let printContents = document.getElementById('contentToExport').innerHTML;
    let originalContents = document.body.innerHTML;
    
    const WindowPrt = window.open('', '', 'left=0,top=0,width=900,height=900,toolbar=0,scrollbars=0,status=0');
    
    WindowPrt.document.write(printContents);
    WindowPrt.focus();
    WindowPrt.print();
    WindowPrt.close();
  }

  exportToPDF() {

    const content = document.getElementById('contentToExport'); // Replace with the ID of your content div

    const pdfOptions = {
      margin: 0,
      filename: 'exported-content.pdf',
      image: { type: 'jpeg', quality: 0.98 },
      html2canvas: { scale: 2 },
      jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
    };
    
    html2pdf().from(content).set(pdfOptions).save();
  }
}

