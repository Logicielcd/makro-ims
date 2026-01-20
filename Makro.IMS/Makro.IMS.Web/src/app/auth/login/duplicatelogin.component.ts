import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { AuthService } from 'auth/auth.service';

@UntilDestroy()
@Component({
  selector: 'app-duplicatelogin',
  templateUrl: './duplicatelogin.component.html',
  styles: [
    `
      :host ::ng-deep .p-password input {
        width: 100%;
        padding: 1rem;
      }

      :host ::ng-deep .pi-eye {
        transform: scale(1.6);
        margin-right: 1rem;
        color: var(--primary-color) !important;
      }

      :host ::ng-deep .pi-eye-slash {
        transform: scale(1.6);
        margin-right: 1rem;
        color: var(--primary-color) !important;
      }
    `,
  ],
})
export class DuplicateLoginComponent {

  constructor(    
    private authService: AuthService,
    public router: Router
  ) {
    
  }

  ngOnInit() {
    
  }

  back(){
    this.router.navigate(['/auth/login']);
  }
}
