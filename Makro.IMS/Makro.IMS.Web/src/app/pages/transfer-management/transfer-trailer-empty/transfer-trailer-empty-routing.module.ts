import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TransferTrailerEmptyComponent } from './transfer-trailer-empty.component';

const routes: Routes = [{ path: '', component: TransferTrailerEmptyComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TransferTrailerEmptyRoutingModule {}
