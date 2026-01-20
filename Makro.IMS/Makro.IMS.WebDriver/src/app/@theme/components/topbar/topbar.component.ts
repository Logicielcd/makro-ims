import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '@core/models/account/user.model';
import { LayoutService } from '@core/services/layout/layout.service';
import { AuthService } from 'auth/auth.service';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
})
export class TopBarComponent {
  items!: MenuItem[];
  user: User;
  constructor(
    public layoutService: LayoutService,
    private authService: AuthService,
    public router: Router
  ) {
    this.user = authService.getUser();
  }

  userLogout() {
    this.authService.userLogout().subscribe((data) => {
      if (data) {
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
