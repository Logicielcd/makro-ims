import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AccordionModule } from 'primeng/accordion';
import { ConfirmationService, MessageService } from 'primeng/api';
import { AutoFocusModule } from 'primeng/autofocus';
import { BadgeModule } from 'primeng/badge';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DataViewModule } from 'primeng/dataview';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { FileUploadModule } from 'primeng/fileupload';
import { InputSwitchModule } from 'primeng/inputswitch';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ListboxModule } from 'primeng/listbox';
import { MenuModule } from 'primeng/menu';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { MultiSelectModule } from 'primeng/multiselect';
import { PanelModule } from 'primeng/panel';
import { PasswordModule } from 'primeng/password';
import { RadioButtonModule } from 'primeng/radiobutton';
import { RippleModule } from 'primeng/ripple';
import { SliderModule } from 'primeng/slider';
import { SplitButtonModule } from 'primeng/splitbutton';
import { StepsModule } from 'primeng/steps';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { TooltipModule } from 'primeng/tooltip';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { TablePageListComponent } from './components/table-page-list/table-page-list.component';
import { StatusModalComponent } from './components/modal/status-modal/status-modal.component';
import { ChipsModule } from 'primeng/chips';
import { QRCodeModule } from 'angularx-qrcode';
import {InputMaskModule} from 'primeng/inputmask';

const PRIME_MODULES = [
  ListboxModule,
  TableModule,
  TooltipModule,
  MultiSelectModule,
  TriStateCheckboxModule,
  CheckboxModule,
  MessagesModule,
  MessageModule,
  InputTextModule,
  InputTextareaModule,
  RadioButtonModule,
  PasswordModule,
  DropdownModule,
  AccordionModule,
  BlockUIModule,
  PanelModule,
  CalendarModule,
  SliderModule,
  StepsModule,
  TagModule,
  SplitButtonModule,
  CardModule,
  ToolbarModule,
  DataViewModule,
  ButtonModule,
  RippleModule,
  ToastModule,
  MenuModule,
  DialogModule,
  AutoFocusModule,
  DynamicDialogModule,
  FileUploadModule,
  ConfirmDialogModule,
  TabViewModule,
  VirtualScrollerModule,
  InputSwitchModule,
  BadgeModule,
  ToggleButtonModule,
  ChipsModule,
  InputMaskModule,
];

const MODULES = [
  CommonModule,
  HttpClientModule,
  FormsModule,
  ReactiveFormsModule,
  TranslateModule,
  RouterModule,
];

const COMPONENTS = [TablePageListComponent];

const MODAL = [
  StatusModalComponent,  
];

const QR_MODULES = [
  QRCodeModule,  
]

@NgModule({
  imports: [...MODULES, ...PRIME_MODULES, ...QR_MODULES],
  exports: [...COMPONENTS, ...MODAL, ...PRIME_MODULES, ...MODULES, ...QR_MODULES],
  declarations: [...COMPONENTS,...MODAL],
  providers: [MessageService, DialogService, ConfirmationService],
})
export class ThemeModule {}
