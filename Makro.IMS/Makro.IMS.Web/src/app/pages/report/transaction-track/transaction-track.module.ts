import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TransactionTrackRoutingModule } from './transaction-track-routing.module';
import { TransactionTrackComponent } from './transaction-track.component';
import { ThemeModule } from '@theme/theme.module';

@NgModule({
  imports: [CommonModule, 
    TransactionTrackRoutingModule,
    ThemeModule
  ],
  declarations: [TransactionTrackComponent,],
})

export class TransactionTrackModule {}
