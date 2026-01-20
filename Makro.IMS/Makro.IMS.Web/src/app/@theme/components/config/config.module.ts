import { NgModule } from '@angular/core';
import { ThemeModule } from '@theme/theme.module';
import { SidebarModule } from 'primeng/sidebar';
import { ConfigComponent } from './config.component';

@NgModule({
  imports: [ThemeModule, SidebarModule],
  declarations: [ConfigComponent],
  exports: [ConfigComponent],
})
export class AppConfigModule {}
