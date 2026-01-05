import { APP_MENU } from '@core/models/menu/app-menu.model';
import { MenuItem } from 'primeng/api';

export const MENU_ITEMS: MenuItem[] = [
  {
    label: 'MAIN MENU',
    items: [
      {
        label: 'Menus.Driver.Title',
        icon: 'pi pi-fw pi-id-card',
        iconStyle: 'color: var(--primary-color); font-size: 1.25rem;',
        state: { module: APP_MENU.driver.module },
        items: [
          {
            label: 'Menus.Driver.GatePass',
            routerLink: ['/pages/driver/gate-pass'],
            state: { page: APP_MENU.driver.gatePass },
          },
          {
            label: 'Menus.Driver.Queue',
            routerLink: ['/pages/driver/queue'],
            state: { page: APP_MENU.driver.queue },
          },
        ],
      },
      {
        label: 'Menus.Account.Title',
        icon: 'pi pi-fw pi-user',
        iconStyle: 'color: var(--primary-color); font-size: 1.25rem;',
        state: { module: APP_MENU.account.module },
        items: [
          {
            label: 'Menus.Account.UserProfile',
            routerLink: ['/pages/account/user-profile'],
            state: { page: APP_MENU.account.userProfile },
          },
          {
            label: 'Menus.Account.SupplierContact',
            routerLink: ['/pages/account/supplier-contact'],
            state: { page: APP_MENU.account.supplierContact },
          },
          {
            label: 'Menus.Account.ResetPassword',
            routerLink: ['/pages/account/reset-password'],
            state: { page: APP_MENU.account.resetPassword },
          },          
        ],
      },
    ],
  },
];
