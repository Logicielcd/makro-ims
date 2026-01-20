import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'gate-pass',
    loadChildren: () =>
      import('./gate-pass/gate-pass.module').then((m) => m.GatePassModule),
  },  
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DriverRoutingModule {}
