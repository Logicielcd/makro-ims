import { Component, OnDestroy, OnInit } from '@angular/core';
import { User } from '@core/models/account/user.model';
import { LayoutService } from '@core/services/layout/layout.service';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { MenuItem } from 'primeng/api';
import { Subscription } from 'rxjs';
import { MENU_ITEMS } from './menu';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
})
export class MenuComponent implements OnInit, OnDestroy {
  private menuItem$ = new Subscription();
  menu: MenuItem[] = [];
  currentUser: User;

  constructor(
    public layoutService: LayoutService,
    private authService: AuthService,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.currentUser = this.authService.userProfile.getValue();
    this.menu = this.translate(MENU_ITEMS);
    this.authMenuItems();
  }

  authMenuItems() {
    if (this.currentUser.authMenus) {
      for (let index = 0; index < this.menu[0].items.length; index++) {
        const item = this.menu[0].items[index];
        if (item) {
          this.authMenuItem(item);
        }
      }
    }
  }

  authMenuItem(menuItem: MenuItem) {
    const authMenus = this.currentUser.authMenus.filter(
      (x) => x.module === menuItem.state.module
    );

    if (authMenus.length == 0) {
      menuItem.visible = false;
    }

    if (authMenus.length > 0 && authMenus[0].pageName === '*') {
      return;
    }

    if (menuItem.items) {
      for (let index = 0; index < menuItem.items.length; index++) {
        const pageMenu = menuItem.items[index];

        if (
          !pageMenu.state ||
          !pageMenu.state.page ||
          !authMenus.map((x) => x.pageName).includes(pageMenu.state.page)
        ) {
          pageMenu.visible = false;
        }
      }
    }
  }

  translate(menuItems: MenuItem[]): MenuItem[] {
    const translationMenu = JSON.parse(JSON.stringify(menuItems));

    for (let mainIndex = 0; mainIndex < menuItems.length; mainIndex++) {
      const mainSubscription = this.translateService
        .stream(menuItems[mainIndex].label)
        .subscribe((res) => {
          const mainMenu = translationMenu[mainIndex];
          mainMenu.label = res;
        });

      this.menuItem$.add(mainSubscription);

      if (menuItems[mainIndex].items) {
        for (
          let itemIndex = 0;
          itemIndex < menuItems[mainIndex].items.length;
          itemIndex++
        ) {
          const childSubscription = this.translateService
            .stream(menuItems[mainIndex].items[itemIndex].label)
            .subscribe((res) => {
              const subMenu = translationMenu[mainIndex].items[itemIndex];
              subMenu.label = res;
            });

          this.menuItem$.add(childSubscription);
        }
      }
    }

    return translationMenu;
  }

  ngOnDestroy() {
    this.menuItem$.unsubscribe();
  }
}
