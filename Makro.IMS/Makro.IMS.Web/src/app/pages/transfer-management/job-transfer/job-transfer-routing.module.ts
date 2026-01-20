import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { JobTransferListComponent } from './list/Job-transfer-list.component';

const routes: Routes = [{ path: '', component: JobTransferListComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class JobTransferRoutingModule {}
