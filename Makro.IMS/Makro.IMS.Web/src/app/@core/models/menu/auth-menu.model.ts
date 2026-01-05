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
      ...AuthMenu.dashboardMenus,
      ...AuthMenu.masterMenus,
      ...AuthMenu.bookingMenus,
      ...AuthMenu.preCheckInMenus,
      ...AuthMenu.checkInOutMenus,
      ...AuthMenu.trasferMenus,
      ...AuthMenu.accountMenus,
    ];
  }

  private static dashboardMenus = [
    new AuthMenu(APP_MENU.dashboard.module, APP_MENU.dashboard.overview),
    new AuthMenu(APP_MENU.dashboard.module, APP_MENU.dashboard.scheduleDoor),
    new AuthMenu(APP_MENU.dashboard.module, APP_MENU.dashboard.summaryBooking),
    new AuthMenu(APP_MENU.dashboard.module, APP_MENU.dashboard.slottimeBooking),
    new AuthMenu(APP_MENU.dashboard.module, APP_MENU.dashboard.dockdoorControl),
  ];

  private static masterMenus = [
    new AuthMenu(APP_MENU.master.module, APP_MENU.master.warehouse),
    new AuthMenu(APP_MENU.master.module, APP_MENU.master.operationType),
    new AuthMenu(APP_MENU.master.module, APP_MENU.master.door),
    new AuthMenu(APP_MENU.master.module, APP_MENU.master.warehousecapacity),
    new AuthMenu(
      APP_MENU.master.module,
      APP_MENU.master.warehouseOperationCapacity
    ),
    new AuthMenu(APP_MENU.master.module, APP_MENU.master.supplierGroup),
    new AuthMenu(APP_MENU.master.module, APP_MENU.master.supplier),
    new AuthMenu(APP_MENU.master.module, APP_MENU.master.estimateTime),
    new AuthMenu(APP_MENU.master.module, APP_MENU.master.yard),
  ];

  private static bookingMenus = [
    new AuthMenu(APP_MENU.booking.module, APP_MENU.booking.createBooking),
    new AuthMenu(APP_MENU.booking.module, APP_MENU.booking.createBookingExcel),
    new AuthMenu(APP_MENU.booking.module, APP_MENU.booking.bookingHeader),
    new AuthMenu(APP_MENU.booking.module, APP_MENU.booking.createBookingNopo),
    new AuthMenu(APP_MENU.booking.module, APP_MENU.booking.checkPo),
    new AuthMenu(APP_MENU.booking.module, APP_MENU.booking.poMonitor),
    new AuthMenu(APP_MENU.booking.module, APP_MENU.booking.capMonitor),
  ];

  private static preCheckInMenus = [
    new AuthMenu(
      APP_MENU.precheckin.module,
      APP_MENU.precheckin.preCheckInExcel
    ),
    new AuthMenu(
      APP_MENU.precheckin.module,
      APP_MENU.precheckin.preCheckInCompleted
    ),
  ];

  private static checkInOutMenus = [
    new AuthMenu(APP_MENU.checkinout.module, APP_MENU.checkinout.guardCheckIn),
    new AuthMenu(APP_MENU.checkinout.module, APP_MENU.checkinout.guardCheckOut),
  ];

  private static queueManageMenus = [
    // new AuthMenu(APP_MENU.queuemanage.module, APP_MENU.queuemanage.documentCheckIn),
    new AuthMenu(APP_MENU.queuemanage.module, APP_MENU.queuemanage.manageQueue),
    new AuthMenu(
      APP_MENU.queuemanage.module,
      APP_MENU.queuemanage.guardCreateBooking
    ),
    // new AuthMenu(APP_MENU.queuemanage.module, APP_MENU.queuemanage.documentConfirmation),
  ];

  private static opsManageMenus = [
    new AuthMenu(APP_MENU.opsmanage.module, APP_MENU.opsmanage.dockdoorList),
  ];

  private static yardMenus = [
    new AuthMenu(
      APP_MENU.yardmanagement.module,
      APP_MENU.yardmanagement.yardBooking
    ),
  ];

  private static reportMenus = [
    new AuthMenu(APP_MENU.report.module, APP_MENU.report.truckStatus),
    new AuthMenu(APP_MENU.report.module, APP_MENU.report.transactionTrack),
  ];

  private static yardMasterMenus = [
    new AuthMenu(APP_MENU.yardmaster.module, APP_MENU.yardmaster.yard),
    new AuthMenu(APP_MENU.yardmaster.module, APP_MENU.yardmaster.trailer),
    new AuthMenu(APP_MENU.yardmaster.module, APP_MENU.yardmaster.shunt),
  ];

  private static trasferMenus = [
    new AuthMenu(APP_MENU.transfer.module, APP_MENU.transfer.yardMonitor),
    new AuthMenu(
      APP_MENU.transfer.module,
      APP_MENU.transfer.transferTrailerEmpty
    ),
    new AuthMenu(
      APP_MENU.transfer.module,
      APP_MENU.transfer.transferTrailerFull
    ),
    new AuthMenu(APP_MENU.transfer.module, APP_MENU.transfer.transferList),
    new AuthMenu(APP_MENU.transfer.module, APP_MENU.transfer.transferProcess),
    new AuthMenu(APP_MENU.transfer.module, APP_MENU.transfer.transferReport),
  ];

  private static accountMenus = [
    new AuthMenu(APP_MENU.account.module, APP_MENU.account.userProfile),
    new AuthMenu(APP_MENU.account.module, APP_MENU.account.supplierContact),
    new AuthMenu(APP_MENU.account.module, APP_MENU.account.resetPassword),
  ];

  static buildMenu(roleMenus: AuthMenu[]): AuthMenu[] {
    const dashboardMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.dashboardMenus))
    );
    const masterMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.masterMenus))
    );
    const bookingMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.bookingMenus))
    );
    const checkInMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.checkInOutMenus))
    );
    const queueMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.queueManageMenus))
    );
    const opsMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.opsManageMenus))
    );
    // const billingMenu = AuthMenu.buildModuleMenu(
    //   roleMenus,
    //   JSON.parse(JSON.stringify(AuthMenu.billingMenus))
    // );
    const reportMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.reportMenus))
    );
    const yardmasterMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.yardMasterMenus))
    );
    const trasferMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.trasferMenus))
    );
    const accountMenu = AuthMenu.buildModuleMenu(
      roleMenus,
      JSON.parse(JSON.stringify(AuthMenu.accountMenus))
    );

    // NOTE: ลำดับเมนูต้องตรงกับ allMenus ด้านบน
    const appMenus = [
      ...dashboardMenu,
      ...masterMenu,
      ...bookingMenu,
      ...checkInMenu,
      ...queueMenu,
      ...opsMenu,
      // ...billingMenu,
      ...reportMenu,
      ...yardmasterMenu,
      ...trasferMenu,
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
