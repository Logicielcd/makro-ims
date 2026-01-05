import { Component, OnInit } from '@angular/core';
import { User } from '@core/models/account/user.model';
import { APP_MENU } from '@core/models/menu/app-menu.model';
import { UserService } from '@core/services/account/user.service';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { Message, SelectItem } from 'primeng/api';
import { DataView } from 'primeng/dataview';

@UntilDestroy()
@Component({
  templateUrl: './user-master-list.component.html',
})
export class UserMasterListComponent implements OnInit {
  dataSource: User[] = [];
  sortOrder: number = 0;
  sortField: string = '';
  sortOptions: SelectItem[] = [];
  filterBy: string = ['userId','userName'].join(',');
  filterApprovedBy: string = ['approved'].join(',');


  msgs: Message[] = [];

  isLoading: boolean = false;
  canModify = true;

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(
    private userService: UserService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private authService: AuthService
  ) {
    this.authService
      .canModify(APP_MENU.account.module, APP_MENU.account.userMaster)
      .pipe(untilDestroyed(this))
      .subscribe((granted) => (this.canModify = granted));
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
  }

  ngOnInit() {
    this.fetchData();

    this.sortOptions = [
      { label: 'Userid ↓', value: 'userId' },
      { label: 'Userid ↑', value: '!userId' },
    ];
  }

  fetchData() {
    this.msgs = [];
    this.isLoading = true;
    this.userService
      .getAll()
      .pipe(untilDestroyed(this))
      .subscribe({
        next: (users) => {
          this.dataSource = users;          
          this.isLoading = false;
        },
        error: (error) => {
          this.msgs = [];
          error.Messages.forEach((msg: any) => {
            this.msgs.push({
              severity: 'error',
              summary: 'Error',
              detail: this.translateService.instant(
                `${this.translatePrefix.FromApi}.${msg}`
              ),
            });
          });
          this.isLoading = false;
        },
      });
  }

  onSortChange(event: any) {
    const value = event.value;
    if (value.indexOf('!') === 0) {
      this.sortOrder = -1;
      this.sortField = value.substring(1, value.length);
    } else {
      this.sortOrder = 1;
      this.sortField = value;
    }
  }

  onFilter(dv: DataView, event: Event) {
    dv.filterBy = this.filterBy;
    dv.filter((event.target as HTMLInputElement).value);
  }

  onApprovedFilter(dv: DataView, event: Event) {
    dv.filterBy = this.filterApprovedBy;
    dv.filter((event.target as HTMLInputElement).value);
  }


  encry(data:string):string{
    return btoa(data);
  }
}
