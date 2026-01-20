import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CheckPoRoutingModule } from './check-po-routing.module';
import { CheckPoComponent } from './check-po.component';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    CheckPoRoutingModule,    
    ThemeModule
  ],
  declarations: [CheckPoComponent,],
})

export class CheckPoModule {}
