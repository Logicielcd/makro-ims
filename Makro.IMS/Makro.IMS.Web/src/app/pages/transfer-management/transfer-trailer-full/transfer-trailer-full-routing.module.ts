import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TransferTrailerFullComponent } from './transfer-trailer-full.component';

const routes: Routes = [{ path: '', component: TransferTrailerFullComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TransferTrailerFullRoutingModule {}
