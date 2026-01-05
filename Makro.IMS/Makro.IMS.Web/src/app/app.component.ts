import { Component } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { AuthService } from 'auth/auth.service';
import { PrimeNGConfig } from 'primeng/api';

@UntilDestroy()
@Component({
  selector: 'app-root',
  template: `<router-outlet></router-outlet>`,
})
export class AppComponent {
  menuMode = 'static';

  constructor(
    private primengConfig: PrimeNGConfig,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.authService.getAccessToken();
    this.primengConfig.ripple = true;
    
    const screenWidth = window.screen.width;
    
    //document.documentElement.style.fontSize = '8px';

    if(screenWidth > 768){
      document.documentElement.style.fontSize = '8px';
     }
     if(screenWidth > 1000){
      document.documentElement.style.fontSize = '9px';
     }
     if(screenWidth > 1500){
       document.documentElement.style.fontSize = '12px';
     }
     if(screenWidth > 1800){
      document.documentElement.style.fontSize = '14px';
    }
  }
}
