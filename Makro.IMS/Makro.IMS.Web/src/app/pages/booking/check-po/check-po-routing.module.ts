import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CheckPoComponent } from './check-po.component';

const routes: Routes = [{ path: '', component: CheckPoComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CheckPoRoutingModule {}
