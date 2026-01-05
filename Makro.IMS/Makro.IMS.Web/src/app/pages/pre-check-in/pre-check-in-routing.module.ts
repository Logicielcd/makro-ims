import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'pre-check-in-excel',
    loadChildren: () =>
      import('./pre-check-in-excel/pre-check-in-excel.module').then((m) => m.PreCheckInExcelModule),    
  },
  {
    path: 'pre-check-in-completed',
    loadChildren: () =>
      import('./pre-check-in-completed/pre-check-in-completed.module').then((m) => m.PreCheckInCompletedModule),    
  },   
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PreCheckInRouterModule {}
