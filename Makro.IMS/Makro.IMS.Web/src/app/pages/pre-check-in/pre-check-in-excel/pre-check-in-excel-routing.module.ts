import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PreCheckInExcelDetailComponent } from './detail/pre-check-in-excel-detail.component';
import { PreCheckInExcelListComponent } from './list/pre-check-in-excel-list.component';

const routes: Routes = [
  { path: '', component: PreCheckInExcelListComponent },
  { path: ':internalHeaderKey', component: PreCheckInExcelDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PreCheckInExcelRoutingModule {}
