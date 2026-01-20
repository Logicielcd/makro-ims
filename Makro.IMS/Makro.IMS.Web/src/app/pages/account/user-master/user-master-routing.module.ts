import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserMasterListComponent } from './list/user-master-list.component';
import { UserMasterDetailComponent } from './detail/user-master-detail.component';


const routes: Routes = [
  { path: '', component: UserMasterListComponent },
  { path: 'create', component: UserMasterDetailComponent },
  { path: ':id', component: UserMasterDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserMasterRoutingModule {}
