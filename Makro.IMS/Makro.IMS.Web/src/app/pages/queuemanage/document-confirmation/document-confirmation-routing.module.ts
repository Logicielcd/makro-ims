import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DocumentConfirmationComponent } from './document-confirmation.component';

const routes: Routes = [
  { path: '', component: DocumentConfirmationComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DocumentConfirmationRoutingModule {}
