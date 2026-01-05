import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DockDoorListComponent } from './dockdoor-list.component';

const routes: Routes = [
  { path: '', component: DockDoorListComponent },
  // { path: ':internalHeaderKey', component: BookingHeaderDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DockDoorListRoutingModule {}
