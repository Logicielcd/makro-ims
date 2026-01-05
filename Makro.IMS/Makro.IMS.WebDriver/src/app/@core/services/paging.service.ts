import { Injectable } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { LazyLoadEvent } from 'primeng/api';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })

export class PagingService {

  private readonly isLoading = new Subject<boolean>();
  readonly isLoading$ = this.isLoading.asObservable();

  pagedData = new Subject<any>();
  exportData = new Subject<any>();
  nested: Map<string, string>;

  curPage: number = 1;
  pageSize: number = 10;

  isBack: boolean = undefined;

  constructor(private router: Router, private route: ActivatedRoute) {    
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.updateCurrentPageFromQueryParams();
      }
    });
  }

  private updateCurrentPageFromQueryParams() {
    const pageQueryParam = this.route.snapshot.queryParamMap.get('page');
    if (pageQueryParam !== null && this.isBack === true) {
      const parsedPage = parseInt(pageQueryParam, 10);
      if (!isNaN(parsedPage) && parsedPage >= 1) {
        this.curPage = parsedPage;
        this.pageSize = +this.route.snapshot.queryParamMap.get('pageSize');
        
      }
    }
    else{
      this.isBack = false;
    }
  }

  private buildFilter(event: LazyLoadEvent): string {

    let filter = '';
    
    Object.entries(event.filters).map(([key, objVal]) => {

      if (objVal.value !== null && objVal.value !== '') {

        let operator = this.matchModeMap(objVal.matchMode);

        if (objVal.matchMode !== 'dateIs' && objVal.matchMode !== 'dateIsNot' && objVal.matchMode !== 'dateAfter') {
        // if (!objVal.matchMode.indexOf('dateIs')) {
          if (this.nested && this.nested.has(key)) {
            filter += `${this.nested.get(key)}${operator}${objVal.value},`;
          } else {
            filter += `${key}${operator}${objVal.value},`;
          }
        } else 
        {
          if(objVal.matchMode === 'dateIs'){
            let dateEnd = new Date(objVal.value);
            dateEnd.setDate(dateEnd.getDate() + 1);
            let endDate =
              dateEnd.getFullYear().toString() +
              '-' +
              (dateEnd.getMonth() + 1).toString().padStart(2, '0') +
              '-' +
              dateEnd.getDate().toString().padStart(2, '0');
            if (this.nested && this.nested.has(key)) {
              filter += `${this.nested.get(key)}>=${objVal.value},`;
              filter += `${this.nested.get(key)}<${endDate},`;
            } else {
              filter += `${key}>=${objVal.value},`;
              filter += `${key}<${endDate},`;
            }
          }
          else if(objVal.matchMode === 'dateIsNot'){
            let dateEnd = new Date(objVal.value);
            dateEnd.setDate(dateEnd.getDate() + 1);
            let endDate =
              dateEnd.getFullYear().toString() +
              '-' +
              (dateEnd.getMonth() + 1).toString().padStart(2, '0') +
              '-' +
              dateEnd.getDate().toString().padStart(2, '0');
            if (this.nested && this.nested.has(key)) {
              filter += `${this.nested.get(key)}<${objVal.value},`;
              filter += `${this.nested.get(key)}>${endDate},`;
            } else {
              filter += `${key}<${objVal.value},`;
              filter += `${key}>${endDate},`;
            }
          }
          else if(objVal.matchMode === 'dateAfter'){
            let dateEnd = new Date(objVal.value);
            dateEnd.setDate(dateEnd.getDate() + 1);
            let endDate =
              dateEnd.getFullYear().toString() +
              '-' +
              (dateEnd.getMonth() + 1).toString().padStart(2, '0') +
              '-' +
              dateEnd.getDate().toString().padStart(2, '0');
            if (this.nested && this.nested.has(key)) {              
              filter += `${this.nested.get(key)}>${endDate},`;
            } else {              
              filter += `${key}>${endDate},`;
            }
          }
        }
      }
      else{
       
      }
    });
    return filter;
  }

  private matchModeMap(matchMode: string): string {
    if (matchMode === 'contains') return '@=';
    if (matchMode === 'equals') return '==';
    if (matchMode === 'startsWith') return '_=';
    if (matchMode === 'dateIs') return '==';
    if (matchMode === 'dateIsNot') return '!=';
    if (matchMode === 'dateBefore') return '<';
    if (matchMode === 'dateAfter') return '>';
  }

  private buildSort(event: LazyLoadEvent): string {
    let sortBy = event.sortField;

    if (this.nested && this.nested.has(sortBy)) {
      sortBy = this.nested.get(sortBy);
    }

    if (event.sortOrder < 0) {
      sortBy = `-${sortBy}`;
    }

    return sortBy;
  }

  setLoading(val: boolean) {
    this.isLoading.next(val);
  }

  loadPage(event: LazyLoadEvent) {
    let currentPage = 0;

    if(this.isBack == true){      
      this.isBack = false;
    }
    else{
      currentPage = (event.first / event.rows < 0 ? event.first : event.first / event.rows) + 1;
      this.curPage = currentPage;
      this.pageSize = event.rows;
    }

    // let sieve = {
    //   page: currentPage,
    //   pageSize: event.rows,
    // };

    let sieve = {
      page: this.curPage,
      pageSize: this.pageSize,
    };

    sieve = Object.assign(sieve, { page: this.curPage });

    if (Object.keys(event.filters).length > 0) {
      const filter = this.buildFilter(event);
      sieve = Object.assign(sieve, { filters: filter });
    }

    if (event.sortField) {
      const sortBy = this.buildSort(event);
      sieve = Object.assign(sieve, { sorts: sortBy });
    }

    this.pagedData.next(sieve);
  }


  loadExport(event: LazyLoadEvent, selected = false) {
    let sieve = {
      selected: selected,
    };

    if (Object.keys(event.filters).length > 0) {
      const filter = this.buildFilter(event);
      sieve = Object.assign(sieve, { filters: filter });
    }

    if (event.sortField) {
      const sortBy = this.buildSort(event);
      sieve = Object.assign(sieve, { sorts: sortBy });
    }
    this.exportData.next(sieve);
  }
}
