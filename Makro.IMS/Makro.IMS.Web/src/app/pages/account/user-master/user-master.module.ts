import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ThemeModule } from '@theme/theme.module';
import { UserMasterRoutingModule } from './user-master-routing.module';
import { UserMasterListComponent } from './list/user-master-list.component';
import { UserMasterDetailComponent } from './detail/user-master-detail.component';


@NgModule({
  imports: [
    ThemeModule,
    UserMasterRoutingModule,
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
    UserMasterListComponent,
    UserMasterDetailComponent,
  ],
})
export class UserMasterModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    'assets/i18n/account/user-profile/',
    '.json'
  );
}
