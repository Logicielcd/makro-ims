import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'yard',
    loadChildren: () => import('./yard/yard.module').then((m) => m.YardModule),
  },
  {
    path: 'shunt',
    loadChildren: () =>
      import('./shunt/shunt.module').then((m) => m.ShuntModule),
  },
  {
    path: 'trailer',
    loadChildren: () =>
      import('./trailer/trailer.module').then((m) => m.TrailerModule),
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class YardMasterRoutingModule {}
