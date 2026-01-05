import {
  AfterContentChecked,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LanguageService } from '@core/services/language.service';
import { PagingService } from '@core/services/paging.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import * as FileSaver from 'file-saver';
import {
  FilterMatchMode,
  LazyLoadEvent, Message,
  SelectItem
} from 'primeng/api';
import { Table } from 'primeng/table';
import { Observable } from 'rxjs';
import { first, take } from 'rxjs/operators';
import * as xlsx from 'xlsx';

@UntilDestroy()
@Component({
  selector: 'app-table-page-list',
  templateUrl: './table-page-list.component.html',
})
export class TablePageListComponent implements OnInit, AfterContentChecked {
  @Input() columns: any;
  @Input() datasource: any[];
  @Input() rows = 10;
  @Input() rowCount = 0;
  @Input() stateKey: string;
  @Input() lookup?: any;
  @Input() createBtn = true;
  @Input() uploadBtn = true;
  @Input() exportBtn = true;
  @Input() selectColumnBtn = true;
  @Input() refreshBtn = true;
  @Input() hasDetail = true;
  @Input() canModify = false;
  @Input() errors: string[] = [];
  @Input() defaultWidth = '260px';
  @Input() title = 'Title.List';
  @Input() fieldDetail = 0;
  @Input() linkDetail = '';
  @Input() showFilter = true;
  @Input() showIndex = false;
  @Input() export$: Observable<any[]>;
  @Input() msgs: Message[] = [];
  @Input() dataKey?: any;
  @Input() showFilterBtn = true;
  @Input() showToolbox = true;
  @Input() showTitle = true;
  @Input() showRemark = false;
  @Input() remark = 'Remark.List';

  first = 0;
  
  selectedValues: any[];

  _selectColumns: any[] = [];
  get selectedColumns(): any[] {
    return this._selectColumns;
  }
  set selectedColumns(value) {
    this._selectColumns = this.columns.filter((x) =>
      value.some((v) => v.header === x.header)
    );
    if (this.stateKey) {
      localStorage.setItem(
        this.stateKey + '-column',
        JSON.stringify(this._selectColumns)
      );
    }
  }

  @Output() uploadEmit = new EventEmitter();
  @Output() exportEmit = new EventEmitter();
  @Output() onPrintData = new EventEmitter();

  isLoading$ = this.pagingService.isLoading$;

  tableClear = 0;

  matchModeOptions: SelectItem[];

  lazyEvent: LazyLoadEvent;

  constructor(
    private pagingService: PagingService,
    private cdref: ChangeDetectorRef,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private route: ActivatedRoute,
    private router: Router,    
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
  }

  ngOnInit() {
    this._selectColumns = this.columns;

    if (this.stateKey) {
            
      let st = JSON.parse(localStorage.getItem(this.stateKey));
      if(!st == null && !st == undefined){
        st.columnWidths = null;
        st.tableWidth = null;
      }
      

      // localStorage.setItem(this.stateKey,st);

      let value = JSON.parse(localStorage.getItem(this.stateKey + '-column'));
      if (value) {
        this._selectColumns = value;
      }
    }

    this.matchModeOptions = [
      { label: 'Date Is', value: FilterMatchMode.DATE_IS },
      // { label: 'Before', value: FilterMatchMode.DATE_BEFORE },
      // { label: 'After', value: FilterMatchMode.DATE_AFTER },
    ];
  }

  ngAfterContentChecked() {
    this.cdref.detectChanges();
  }

  loadData(event: LazyLoadEvent) {
    if (event) {
      this.lazyEvent = event;
    }

    this.pagingService.loadPage(this.lazyEvent);
  }

  exportSelected() {
    this.pagingService.loadExport(this.lazyEvent, true);
  }

  exportAll() {
    this.pagingService.loadExport(this.lazyEvent);
  }

  clearFilter(table: Table) {
    localStorage.removeItem(this.stateKey);
    table.clear();
  }


  exportExcel(value: any[], selected = false) {
   
    if (value.length > 0) {
      const headers = selected
        ? this._selectColumns.map((x) => x.header)
        : this.columns.map((x) => x.header);

      // 1. สร้างข้อมูลใหม่ที่สอดคล้องกับหัวของคอลัมน์
      const mappedData = value.map((item) => {
        const newItem: any = {};
        headers.forEach((header, index) => {
          newItem[header] =
            item[
              selected
                ? this._selectColumns[index].field
                : this.columns[index].field
            ];
        });
        return newItem;
      });

      // 2. อัปเดตข้อมูลในตัวแปร data
      const data = [...mappedData];

      const worksheet = xlsx.utils.json_to_sheet(data);

      // 3. ตั้งค่าหัวของคอลัมน์ในแผ่นงาน Excel
      this.setHeader(worksheet, headers);

      const workbook = {
        Sheets: { data: worksheet },
        SheetNames: ['data'],
      };
      const excelBuffer: any = xlsx.write(workbook, {
        bookType: 'xlsx',
        type: 'array',
      });
      this.saveAsExcelFile(excelBuffer, this.stateKey);
    }
  }

  // exportExcel(value: any[], selected = false) {
  //   if (value.length > 0) {
  //     const headers = selected
  //       ? this._selectColumns.map((x) => x.header)
  //       : this.columns.map((x) => x.header);
  //     const fields = selected
  //       ? this._selectColumns.map((x) => x.field)
  //       : this.columns.map((x) => x.field);
  //     const data = [...value];
  //     Object.keys(data[0]).forEach((prop) =>
  //       !fields.includes(prop) ? data.filter((x) => delete x[prop]) : null
  //     );
  //     const worksheet = xlsx.utils.json_to_sheet(data);
  //     this.setHeader(worksheet, headers);

  //     const workbook = {
  //       Sheets: { data: worksheet },
  //       SheetNames: ['data'],
  //     };
  //     const excelBuffer: any = xlsx.write(workbook, {
  //       bookType: 'xlsx',
  //       type: 'array',
  //     });
  //     this.saveAsExcelFile(excelBuffer, this.stateKey);
  //   }
  // }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const EXCEL_TYPE =
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE,
    });
    FileSaver.saveAs(
      data,
      fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION
    );
  }

  private setHeader(ws: xlsx.WorkSheet, header: string[]): void {
    for (let index = 0; index < header.length; index++) {
      const address = xlsx.utils.encode_col(index) + '1';

      if (!ws[address]) {
        continue;
      }

      ws[address].v = this.translateService.instant(header[index]);
    }
  }

  private encry(id:any){
    const encodedId = btoa(id);
    return encodedId;
  }

  goToDetail(itemId: any): void {
    // Capture the current pagination state before navigating to the detail page.
    const { curPage, pageSize } = this.pagingService;
    const encodedId = btoa(itemId);
    // Navigate to the detail page with pagination state preserved in the query parameters.
    this.router.navigate([encodedId], {
      relativeTo: this.route,
      queryParams: { page: curPage, pageSize },
    });
  }

}
