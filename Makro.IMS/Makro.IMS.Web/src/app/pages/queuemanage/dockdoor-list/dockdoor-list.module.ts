import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
// import { BookingHeaderDetailComponent } from './detail/booking-header-detail.component';
// import { BookingHeaderListComponent } from './list/booking-header-list.component';
// import { SlotTimeDtlComponent } from './detail/dialog/slot-time-dtl.component';
import { DockDoorListComponent } from './dockdoor-list.component';
import { DockDoorListRoutingModule } from './dockdoor-list-routing.module';
import { CallTruckDialogComponent } from './call-truck-dialog/call-truck-dialog.component';

@NgModule({
  imports: [
    ThemeModule,
    DockDoorListRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    })
  ],
  declarations: [
    DockDoorListComponent,
    CallTruckDialogComponent,
    // BookingHeaderListComponent,    
    // SlotTimeDtlComponent,
  ],
})
export class DockDoorListModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/queuemanage/dockdoor-list/',
    '.json'
  );
}
