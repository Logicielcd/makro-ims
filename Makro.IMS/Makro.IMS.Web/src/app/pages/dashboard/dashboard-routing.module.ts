import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { 
    path: 'schedule-door', 
    loadChildren: () =>
    import('./schedule-door/schedule-door.module').then((m)=>m.ScheduleDoorModule),    
  },
  { 
    path: 'summary-booking', 
    loadChildren: () =>
    import('./summary-booking/summary-booking.module').then((m)=>m.SummaryBookingModule),    
  },
  { 
    path: 'slottime-booking', 
    loadChildren: () =>
    import('./slottime-booking/slottime-booking.module').then((m)=>m.SlottimeBookingModule),    
  },
  { 
    path: 'dockdoor-control', 
    loadChildren: () =>
    import('./dockdoor-control/dockdoor-control.module').then((m)=>m.DockDoorControlModule),    
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DashboardsRoutingModule {}
