import { APP_MENU } from '@core/models/menu/app-menu.model';
import { MenuItem } from 'primeng/api';

export const MENU_ITEMS: MenuItem[] = [
  {
    label: 'MAIN MENU',
    items: [
      {
        label: 'Menus.Dashboard.Title',
        icon: 'pi pi-fw pi-chart-bar',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.dashboard.module },
        items: [
          {
            label: 'Menus.Dashboard.ScheduleDoor',
            routerLink: ['/pages/dashboard/schedule-door'],
            state: { page: APP_MENU.dashboard.scheduleDoor },
          },
          {
            label: 'Menus.Dashboard.SummaryBooking',
            routerLink: ['/pages/dashboard/summary-booking'],
            state: { page: APP_MENU.dashboard.summaryBooking },
          },
          {
            label: 'Menus.Dashboard.SlottimeBooking',
            routerLink: ['/pages/dashboard/slottime-booking'],
            state: { page: APP_MENU.dashboard.slottimeBooking },
          },
          {
            label: 'Menus.Dashboard.DockdoorControl',
            routerLink: ['/pages/dashboard/dockdoor-control'],
            state: { page: APP_MENU.dashboard.slottimeBooking },
          },
        ],
      },
      {
        label: 'Menus.Master.Title',
        icon: 'pi pi-fw pi-cog',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.master.module },
        items: [
          {
            label: 'Menus.Master.Warehouse',
            routerLink: ['/pages/master/warehouse'],
            state: { page: APP_MENU.master.warehouse },
          },
          {
            label: 'Menus.Master.OperationType',
            routerLink: ['/pages/master/operation-type'],
            state: { page: APP_MENU.master.operationType },
          },
          {
            label: 'Menus.Master.Door',
            routerLink: ['/pages/master/door'],
            state: { page: APP_MENU.master.door },
          },
          {
            label: 'Menus.Master.WarehouseCapacity',
            routerLink: ['/pages/master/warehouse-capacity'],
            state: { page: APP_MENU.master.warehousecapacity },
          },
          {
            label: 'Menus.Master.WarehouseOperationCapacity',
            routerLink: ['/pages/master/warehouse-operation-capacity'],
            state: { page: APP_MENU.master.warehouseOperationCapacity },
          },
          {
            label: 'Menus.Master.SupplierGroup',
            routerLink: ['/pages/master/supplier-group'],
            state: { page: APP_MENU.master.supplierGroup },
          },
          {
            label: 'Menus.Master.Supplier',
            routerLink: ['/pages/master/supplier'],
            state: { page: APP_MENU.master.supplier },
          },
          {
            label: 'Menus.Master.Supplier',
            routerLink: ['/pages/master/estimate-time'],
            state: { page: APP_MENU.master.estimateTime },
          },
        ],
      },
      {
        label: 'Menus.Booking.Title',
        icon: 'pi pi-fw pi-calendar-plus',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.booking.module },
        items: [
          {
            label: 'Menus.Booking.CreateBooking',
            routerLink: ['/pages/booking/create-booking'],
            state: { page: APP_MENU.booking.createBooking },
          },
          {
            label: 'Menus.Booking.CreateBookingExcel',
            routerLink: ['/pages/booking/create-booking-excel'],
            state: { page: APP_MENU.booking.createBookingExcel },
          },
          {
            label: 'Menus.Booking.BookingList',
            routerLink: ['/pages/booking/booking-header'],
            state: { page: APP_MENU.booking.bookingHeader },
          },
          {
            label: 'Menus.Booking.CreateBookingNopo',
            routerLink: ['/pages/booking/create-booking-nopo'],
            state: { page: APP_MENU.booking.createBookingNopo },
          },
          {
            label: 'Menus.Booking.CheckPo',
            routerLink: ['/pages/booking/check-po'],
            state: { page: APP_MENU.booking.checkPo },
          },
          {
            label: 'Menus.Booking.PoMonitor',
            routerLink: ['/pages/booking/po-monitor'],
            state: { page: APP_MENU.booking.poMonitor },
          },
          {
            label: 'Menus.Booking.CapMonitor',
            routerLink: ['/pages/booking/cap-monitor'],
            state: { page: APP_MENU.booking.capMonitor },
          },
        ],
      },
      {
        label: 'Menus.PreCheckIn.Title',
        icon: 'pi pi-fw pi-id-card',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.precheckin.module },
        items: [
          {
            label: 'Menus.PreCheckIn.PreCheckInExcel',
            routerLink: ['/pages/pre-check-in/pre-check-in-excel'],
            state: { page: APP_MENU.precheckin.preCheckInExcel },
          },
          {
            label: 'Menus.PreCheckIn.PreCheckInCompleted',
            routerLink: ['/pages/pre-check-in/pre-check-in-completed'],
            state: { page: APP_MENU.precheckin.preCheckInCompleted },
          },
        ],
      },
      {
        label: 'Menus.CheckInOut.Title',
        icon: 'pi pi-fw pi-check-square',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.checkinout.module },
        items: [
          {
            label: 'Menus.CheckInOut.GuardCheckIn',
            routerLink: ['/pages/check-in-out/guard-check-in'],
            state: { page: APP_MENU.checkinout.guardCheckIn },
          },
          {
            label: 'Menus.CheckInOut.GuardCheckOut',
            routerLink: ['/pages/check-in-out/guard-check-out'],
            state: { page: APP_MENU.checkinout.guardCheckOut },
          },
        ],
      },
      {
        label: 'Menus.Queuemanage.Title',
        icon: 'pi pi-fw pi-desktop',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.queuemanage.module },
        items: [
          {
            label: 'Menus.Queuemanage.ManageQueue',
            routerLink: ['/pages/queuemanage/manage-queue'],
            state: { page: APP_MENU.queuemanage.manageQueue },
          },
          {
            label: 'Menus.CheckInOut.GuardCreateBooking',
            routerLink: ['/pages/check-in-out/guard-create-booking'],
            state: { page: APP_MENU.queuemanage.guardCreateBooking },
          },
        ],
      },
      {
        label: 'Menus.Opsmanage.Title',
        icon: 'pi pi-fw pi-truck',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.opsmanage.module },
        items: [
          {
            label: 'Menus.Opsmanage.DockdoorList',
            routerLink: ['/pages/opsmanage/dockdoor-list'],
            state: { page: APP_MENU.opsmanage.dockdoorList },
          },
        ],
      },
      {
        label: 'Menus.Yardmanagement.Title',
        icon: 'pi pi-fw pi-truck',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.yardmanagement.module },
        items: [
          {
            label: 'Menus.YardManagement.YardBooking',
            routerLink: ['/pages/yard-management/yard-booking'],
            state: { page: APP_MENU.yardmanagement.yardBooking },
          },
        ],
      },
      {
        label: 'Menus.Report.Title',
        icon: 'pi pi-fw pi-book',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.report.module },
        items: [
          {
            label: 'Menus.Report.TruckStatus',
            routerLink: ['/pages/report/truck-status'],
            state: { page: APP_MENU.report.truckStatus },
          },
          {
            label: 'Menus.Report.TransactionTrack',
            routerLink: ['/pages/report/transaction-track'],
            state: { page: APP_MENU.report.transactionTrack },
          },
        ],
      },
      {
        label: 'Menus.YardMaster.Title',
        icon: 'pi pi-fw pi-box',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.yardmaster.module },
        items: [
          {
            label: 'Menus.YardMaster.Shunt',
            routerLink: ['/pages/yard-master/shunt'],
            state: { page: APP_MENU.yardmaster.shunt },
          },
          {
            label: 'Menus.YardMaster.Trailer',
            routerLink: ['/pages/yard-master/trailer'],
            state: { page: APP_MENU.yardmaster.trailer },
          },
          {
            label: 'Menus.YardMaster.Yard',
            routerLink: ['/pages/yard-master/yard'],
            state: { page: APP_MENU.yardmaster.yard },
          },
        ],
      },
      {
        label: 'Menus.Transfer.Title',
        icon: 'pi pi-fw pi-sync',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.transfer.module },
        items: [
          {
            label: 'Menus.Transfer.YardMonitor',
            routerLink: ['/pages/transfer/yard-monitor'],
            state: { page: APP_MENU.transfer.yardMonitor },
          },
          {
            label: 'Menus.Transfer.TrailerMonitor',
            routerLink: ['/pages/transfer/trailer-monitor'],
            state: { page: APP_MENU.transfer.trailerMonitor },
          },
          // {
          //   label: 'Menus.Transfer.TransferTrailerEmpty',
          //   routerLink: ['/pages/transfer/transfer-trailer-empty'],
          //   state: { page: APP_MENU.transfer.transferTrailerEmpty },
          // },
          // {
          //   label: 'Menus.Transfer.TransferTrailerFull',
          //   routerLink: ['/pages/transfer/transfer-trailer-full'],
          //   state: { page: APP_MENU.transfer.transferTrailerFull },
          // },
          {
            label: 'Menus.Transfer.JobList',
            routerLink: ['/pages/transfer/job-transfer'],
            state: { page: APP_MENU.transfer.transferList },
          },
          {
            label: 'Menus.Transfer.JobDockDoor',
            routerLink: ['/pages/transfer/job-dock-door'],
            state: { page: APP_MENU.transfer.transferList },
          },
        ],
      },
      {
        label: 'Menus.Account.Title',
        icon: 'pi pi-fw pi-user',
        iconStyle: {
          color: 'var(--primary-color)',
          'font-size': '1.25rem',
        },
        state: { module: APP_MENU.account.module },
        items: [
          {
            label: 'Menus.Account.UserMaster',
            routerLink: ['/pages/account/user-master'],
            state: { page: APP_MENU.account.userMaster },
          },
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
