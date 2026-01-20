import {
  Component,
  OnDestroy,
  OnInit,
  Renderer2,
  ViewChild
} from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { LayoutService } from '@core/services/layout/layout.service';
// import { DepartmentService } from '@core/services/master/department.service';
import { MenuService } from '@core/services/menu/app.menu.service';
import { UntilDestroy } from '@ngneat/until-destroy';
import { SidebarComponent } from '@theme/components/sidebar/sidebar.component';
import { AuthService } from 'auth/auth.service';
import { MenuItem, Message, MessageService,ConfirmationService } from 'primeng/api';
import { filter, Observable, Subscription, tap } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
})
export class LayoutComponent implements OnInit, OnDestroy {
  overlayMenuOpenSubscription: Subscription;

  menuOutsideClickListener: any;

  msgs: Message[] = [];

  // deps$: Observable<Department[]>;

  @ViewChild(SidebarComponent) appSidebar!: SidebarComponent;

  constructor(
    private menuService: MenuService,
    // private departmentService: DepartmentService,
    private authService: AuthService,
    public layoutService: LayoutService,
    public renderer: Renderer2,
    public router: Router,
    private messageService: MessageService
  ) {
    this.overlayMenuOpenSubscription =
      this.layoutService.overlayOpen$.subscribe(() => {
        if (!this.menuOutsideClickListener) {
          this.menuOutsideClickListener = this.renderer.listen(
            'document',
            'click',
            (event) => {
              const isOutsideClicked = !(
                this.appSidebar.el.nativeElement.isSameNode(event.target) ||
                this.appSidebar.el.nativeElement.contains(event.target) ||
                event.target.classList.contains('p-trigger') ||
                event.target.parentNode.classList.contains('p-trigger')
              );

              if (isOutsideClicked) {
                this.layoutService.state.profileSidebarVisible = false;
                this.layoutService.state.overlayMenuActive = false;
                this.layoutService.state.staticMenuMobileActive = false;
                this.layoutService.state.menuHoverActive = false;
                this.menuService.reset();
                this.menuOutsideClickListener();
                this.menuOutsideClickListener = null;
                this.unblockBodyScroll();
              } else {
                if (this.layoutService.state.staticMenuMobileActive) {
                  this.blockBodyScroll();
                }
              }
            }
          );
        }
      });

    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        this.unblockBodyScroll();
      });
  }

  ngOnInit(): void {

    const user = this.authService.userProfile.getValue();

    if(user.authMenus.length === 1)
    {
      this.msgs.push({
        severity: 'error',
        summary: 'Password expired',
        detail: 'Please change your password and login again.',
      });
    }

    // this.deps$ = this.departmentService.getAll().pipe(
    //   tap((data) => {
    //     const deps = data.filter((x) => user.depIds.includes(x.id));
    //     this.authService.setDeps(deps);
    //     const department = deps.find((x) => x.id == user.defaultDepartment);
    //     this.authService.selectDep(department);
    //   })
    // );
  }

  blockBodyScroll(): void {
    if (document.body.classList) {
      document.body.classList.add('blocked-scroll');
    } else {
      document.body.className += ' blocked-scroll';
    }
  }

  unblockBodyScroll(): void {
    if (document.body.classList) {
      document.body.classList.remove('blocked-scroll');
    } else {
      document.body.className = document.body.className.replace(
        new RegExp(
          '(^|\\b)' + 'blocked-scroll'.split(' ').join('|') + '(\\b|$)',
          'gi'
        ),
        ' '
      );
    }
  }

  get containerClass() {
    return {
      'layout-theme-light': this.layoutService.config.colorScheme === 'light',
      'layout-theme-dark': this.layoutService.config.colorScheme === 'dark',
      'layout-overlay': this.layoutService.config.menuMode === 'overlay',
      'layout-static': this.layoutService.config.menuMode === 'static',
      'layout-slim': this.layoutService.config.menuMode === 'slim',
      'layout-horizontal': this.layoutService.config.menuMode === 'horizontal',
      'layout-static-inactive':
        this.layoutService.state.staticMenuDesktopInactive &&
        this.layoutService.config.menuMode === 'static',
      'layout-overlay-active': this.layoutService.state.overlayMenuActive,
      'layout-mobile-active': this.layoutService.state.staticMenuMobileActive,
      'p-input-filled': this.layoutService.config.inputStyle === 'filled',
      'p-ripple-disabled': !this.layoutService.config.ripple,
    };
  }

  ngOnDestroy() {
    if (this.overlayMenuOpenSubscription) {
      this.overlayMenuOpenSubscription.unsubscribe();
    }

    if (this.menuOutsideClickListener) {
      this.menuOutsideClickListener();
    }
  }
}
