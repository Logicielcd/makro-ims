import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [  
  {
    path: 'dockdoor-list',
    loadChildren: () =>
      import('./dockdoor-list/dockdoor-list.module').then((m) => m.DockDoorListModule),
  },      
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OpsManageRoutingModule {}
