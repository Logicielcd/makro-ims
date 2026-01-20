import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { 
    path: 'truck-status', 
    loadChildren: () =>
    import('./truck-status/truck-status.module').then((m)=>m.TruckStatusModule),    
  },
  { 
    path: 'transaction-track', 
    loadChildren: () =>
    import('./transaction-track/transaction-track.module').then((m)=>m.TransactionTrackModule),    
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportRoutingModule {}
