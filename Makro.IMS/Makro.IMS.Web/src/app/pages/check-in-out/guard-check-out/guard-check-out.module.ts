import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { GuardCheckOutRoutingModule } from './guard-check-out-routing.module';
import { GuardCheckOutComponent } from './guard-check-out.component';
import { DialogComponent } from './dialog/dialog.component';

@NgModule({
  imports: [
    ThemeModule,
    GuardCheckOutRoutingModule,
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
    GuardCheckOutComponent,
    DialogComponent,
  ],
})
export class GuardCheckOutModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/check-in-out/guard-check-out/',
    '.json'
  );
}
