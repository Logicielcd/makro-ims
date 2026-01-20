import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DocumentCheckInComponent } from './document-check-in.component';

const routes: Routes = [
  { path: '', component: DocumentCheckInComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DocumentCheckInRoutingModule {}
