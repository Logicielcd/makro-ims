import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TransactionTrackComponent } from './transaction-track.component';

const routes: Routes = [{ path: '', component: TransactionTrackComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TransactionTrackRoutingModule {}
