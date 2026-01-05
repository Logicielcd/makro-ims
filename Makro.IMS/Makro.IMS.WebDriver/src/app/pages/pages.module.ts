import { NgModule } from '@angular/core';
import { LayoutModule } from '@theme/layouts/layout.module';
import { ThemeModule } from '../@theme/theme.module';
import { PagesRoutingModule } from './pages-routing.module';
import { PagesComponent } from './pages.component';

@NgModule({
  imports: [ThemeModule, PagesRoutingModule, LayoutModule],
  declarations: [PagesComponent],
})
export class PagesModule {}
