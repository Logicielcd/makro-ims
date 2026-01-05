import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { LanguageService } from '@core/services/language.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { MenuItem, Message, MessageService,ConfirmationService } from 'primeng/api';

/* service */
import { SupplierService } from '@core/services/master/supplier.service';

/* model */
import { User } from '@core/models/account/user.model';

@UntilDestroy()
@Component({
  templateUrl: './user-profile.component.html',
})
export class UserProfileComponent implements OnInit {

  title = 'User Profile';

  user: User;
  data: User;

  items: MenuItem[];
  msgs: Message[] = [];

  tabIndex: number = 0;

  confirmDialog: boolean = false;
  isLoading: boolean = false;

  translatePrefix = {
    FromApi: 'Message.FromApi',
  };

  constructor(    
    private authService: AuthService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private languageService: LanguageService,
    private translateService: TranslateService,
    private supplierService: SupplierService,
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });

  }

  ngOnInit() {
    this.user = this.authService.getUser();
    this.data = this.user;
    console.log(this.user);
  }


}