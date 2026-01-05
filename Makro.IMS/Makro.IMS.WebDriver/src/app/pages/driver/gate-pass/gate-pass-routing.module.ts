import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GatePassComponent } from './gate-pass.component';

const routes: Routes = [
  { path: '', component: GatePassComponent },  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GatePassRoutingModule {}
