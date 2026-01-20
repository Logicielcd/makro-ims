import { APP_MENU } from './app-menu.model';

export enum PageAction {
  View = 'View',
  Modify = 'Modify',
  Disable = 'Disable',
}

export class AuthMenu {
  constructor(
    public module: string,
    public pageName: string,
    public action: PageAction = PageAction.Disable,
    public writeOnly = false
  ) {
    this.module = module;
    this.pageName = pageName;
    this.action = action;
    this.writeOnly = writeOnly;
  }

  // NOTE: ลำดับเมนูต้องตรงกับ appMenus ใน buildMenu
  static get allMenu(): AuthMenu[] {
    return [
      ...AuthMenu.driverMenus,      
      ...AuthMenu.accountMenus,
    ];
  }

  private static driverMenus = [    
    new AuthMenu(APP_MENU.driver.module, APP_MENU.driver.gatePass),
    // new AuthMenu(APP_MENU.driver.module, APP_MENU.driver.queue),      
  ];


  private static accountMenus = [
    new AuthMenu(APP_MENU.account.module, APP_MENU.account.userProfile),
    new AuthMenu(APP_MENU.account.module, APP_MENU.account.supplierContact),
    new AuthMenu(APP_MENU.account.module, APP_MENU.account.resetPassword),    
  ];

  static buildMenu(roleMenus: AuthMenu[]): AuthMenu[] {
    const driverMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.driverMenus))
    );
    const accountMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.accountMenus))
    );

    // NOTE: ลำดับเมนูต้องตรงกับ allMenus ด้านบน
    const appMenus = [
      ...driverMenu,
      ...accountMenu,
    ];

    return appMenus;
  }

  static buildModuleMenu(
    roleMenus: AuthMenu[],
    moduleMenus: AuthMenu[]
  ): AuthMenu[] {
    const moduleName = moduleMenus[0].module;

    // to remove matched menu from role menus, https://gist.github.com/chad3814/2924672.
    for (const roleMenu of roleMenus) {
      if (moduleName === roleMenu.module && roleMenu.pageName === '*') {
        moduleMenus.forEach((menu) => (menu.action = roleMenu.action));
      } else if (moduleName === roleMenu.module) {
        moduleMenus.forEach((menu) => {
          if (menu.pageName === roleMenu.pageName) {
            menu.action = roleMenu.action;
          }
        });
      }
    }

    return moduleMenus;
  }

  /* grouping array
  // https://www.consolelog.io/group-by-in-javascript/
  */
  static parseMenu(menus: AuthMenu[]): AuthMenu[] {
    // group to object, { <module>: <[{ page1, action }, { page2, action }]> }
    const group = menus.reduce((obj, item) => {
      obj[item.module] = obj[item.module] || [];
      obj[item.module].push({ page: item.pageName, action: item.action });
      return obj;
    }, {});

    // to convert to array of { module: <x>, pages: <[{ name: y, action: z }]> }, https://stackoverflow.com/a/31689499/580844
    // const groupedMenu = Object.keys(group).map(module => ({ module: module, pages: group[module] }));

    const result: AuthMenu[] = [];
    // if all pages in module are authorized, page name will be '*' to reduce the token length.
    for (const module in group) {
      if (module) {
        const viewCount = group[module].reduce(
          (initVal: number, page: { action: PageAction }) =>
            initVal + +(page.action === PageAction.View),
          0
        );
        const modifyCount = group[module].reduce(
          (initVal: number, page: { action: PageAction }) =>
            initVal + +(page.action === PageAction.Modify),
          0
        );

        if (viewCount === group[module].length) {
          result.push(new AuthMenu(module, '*', PageAction.View));
        } else if (modifyCount === group[module].length) {
          result.push(new AuthMenu(module, '*', PageAction.Modify));
        } else {
          for (const menu of group[module]) {
            if (menu.action !== PageAction.Disable) {
              result.push(new AuthMenu(module, menu.page, menu.action));
            }
          }
        }
      }
    }

    return result;
  }
}
