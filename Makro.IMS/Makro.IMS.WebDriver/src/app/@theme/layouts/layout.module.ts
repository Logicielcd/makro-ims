import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { AppConfigModule } from '@theme/components/config/config.module';
import { MenuComponent } from '@theme/components/menu/menu.component';
import { MenuitemComponent } from '@theme/components/menuitem/menuitem.component';
import { SidebarComponent } from '@theme/components/sidebar/sidebar.component';
import { TopBarComponent } from '@theme/components/topbar/topbar.component';
import { ThemeModule } from '@theme/theme.module';
import { SidebarModule } from 'primeng/sidebar';
import { LayoutComponent } from './layout.component';

@NgModule({
  declarations: [
    MenuitemComponent,
    TopBarComponent,
    MenuComponent,
    SidebarComponent,
    LayoutComponent,
  ],
  imports: [
    ThemeModule,
    RouterModule,
    SidebarModule,
    AppConfigModule,
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  exports: [LayoutComponent],
})
export class LayoutModule {}

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, 'assets/i18n/', '.json');
}
