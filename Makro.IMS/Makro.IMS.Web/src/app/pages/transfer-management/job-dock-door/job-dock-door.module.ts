import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { JobDockDoorRoutingModule } from './job-dock-door-routing.module';
import { JobDockDoorListComponent } from './list/job-dock-door-list.component';
import { ChangeDoorModalComponent } from './change-door-modal/change-door-modal.component';
import { ChangeTrailerModalComponent } from './change-trailer-modal/change-trailer-modal.component';

@NgModule({
  imports: [
    ThemeModule,
    JobDockDoorRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  declarations: [
    JobDockDoorListComponent,
    ChangeDoorModalComponent,
    ChangeTrailerModalComponent,
  ],
})
export class JobDockDoorModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/transfer-management/job-dock-door/',
    '.json'
  );
}
