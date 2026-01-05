import { Component, Input, OnInit } from '@angular/core';
import { Translate } from '@core/models/account/translate.model';
import { User } from '@core/models/account/user.model';
import { UserService } from '@core/services/account/user.service';
import { LanguageService } from '@core/services/language.service';
import { LayoutService } from '@core/services/layout/layout.service';
// import { TransporterService } from '@core/services/master/transporter.service';
// import { VehicleService } from '@core/services/master/vehicle.service';
import { MenuService } from '@core/services/menu/app.menu.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'auth/auth.service';
import { Observable } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'app-config',
  templateUrl: './config.component.html',
})
export class ConfigComponent implements OnInit {
  @Input() minimal: boolean = false;
  // departments: Department[];
  translates$: Observable<Translate[]>;
  // transporter: Transporter;
  // mainVehicle: Vehicle;
  // secondaryVehicle: Vehicle;
  translates: Translate[];

  scales: number[] = [12, 13, 14, 15, 16];

  // selectedDepartment: Department;

  userProfile?: User | null;

  constructor(
    public layoutService: LayoutService,
    public menuService: MenuService,
    private userService: UserService,
    // private transporterService: TransporterService,
    // private vehicleService: VehicleService,
    private authService: AuthService,
    private languageService: LanguageService,
    private translateService: TranslateService
  ) {
    this.languageService.language$
      .pipe(untilDestroyed(this))
      .subscribe((language: string) => {
        this.translateService.use(language);
      });
  }

  ngOnInit(): void {
    this.userProfile = this.authService.userProfile.getValue();
    this.translates$ = this.userService.getTranslate();

    // this.authService.availableDeps$
    //   .pipe(untilDestroyed(this))
    //   .subscribe((data) => (this.departments = data));

    // this.translates$ = this.userService.getTranslate();

    // if (this.userProfile.transpoterId) {
    //   this.transporterService
    //     .getById(this.userProfile.transpoterId)
    //     .pipe(untilDestroyed(this))
    //     .subscribe((data) => (this.transporter = data));
    // }

    // if (this.userProfile.mainVehicleId) {
    //   this.vehicleService
    //     .getById(this.userProfile.mainVehicleId)
    //     .pipe(untilDestroyed(this))
    //     .subscribe((data) => (this.mainVehicle = data));
    // }

    // if (this.userProfile.secondaryVehicleId) {
    //   this.vehicleService
    //     .getById(this.userProfile.secondaryVehicleId)
    //     .pipe(untilDestroyed(this))
    //     .subscribe((data) => (this.secondaryVehicle = data));
    // }
  }

  // changeDep(departmentId: string) {
  //   this.userProfile.defaultDepartment = departmentId;
  //   const department = this.departments.find((x) => x.id == departmentId);
  //   // this.authService.selectDep(department);
  //   this.userService
  //     .updateConfig(this.userProfile)
  //     .pipe(untilDestroyed(this))
  //     .subscribe((user) => this.authService.setUser(user));
  // }

  changeTranslate(language: string) {
    this.userProfile.translate = language;
    this.languageService.setLanguage(language);
    this.userService
      .updateConfig(this.userProfile)
      .pipe(untilDestroyed(this))
      .subscribe((user) => this.authService.setUser(user));
  }

  get visible(): boolean {
    return this.layoutService.state.configSidebarVisible;
  }

  set visible(_val: boolean) {
    this.layoutService.state.configSidebarVisible = _val;
  }

  get scale(): number {
    return this.layoutService.config.scale;
  }

  set scale(_val: number) {
    this.layoutService.config.scale = _val;
  }

  get menuMode(): string {
    return this.layoutService.config.menuMode;
  }

  set menuMode(_val: string) {
    this.layoutService.config.menuMode = _val;
    if (this.layoutService.isSlim()) {
      this.menuService.reset();
    }
  }

  get inputStyle(): string {
    return this.layoutService.config.inputStyle;
  }

  set inputStyle(_val: string) {
    this.layoutService.config.inputStyle = _val;
  }

  get ripple(): boolean {
    return this.layoutService.config.ripple;
  }

  set ripple(_val: boolean) {
    this.layoutService.config.ripple = _val;
  }

  onConfigButtonClick() {
    this.layoutService.showConfigSidebar();
  }

  changeTheme(theme: string, colorScheme: string) {
    const themeLink = <HTMLLinkElement>document.getElementById('theme-css');
    const newHref = themeLink
      .getAttribute('href')!
      .replace(this.layoutService.config.theme, theme);
    this.layoutService.config.colorScheme;
    this.replaceThemeLink(newHref, () => {
      this.layoutService.config.theme = theme;
      this.layoutService.config.colorScheme = colorScheme;
      this.layoutService.onConfigUpdate();
    });
  }

  replaceThemeLink(href: string, onComplete: Function) {
    const id = 'theme-css';
    const themeLink = <HTMLLinkElement>document.getElementById('theme-css');
    const cloneLinkElement = <HTMLLinkElement>themeLink.cloneNode(true);

    cloneLinkElement.setAttribute('href', href);
    cloneLinkElement.setAttribute('id', id + '-clone');

    themeLink.parentNode!.insertBefore(cloneLinkElement, themeLink.nextSibling);

    cloneLinkElement.addEventListener('load', () => {
      themeLink.remove();
      cloneLinkElement.setAttribute('id', id);
      onComplete();
    });
  }

  decrementScale() {
    this.scale--;
    this.applyScale();
  }

  incrementScale() {
    this.scale++;
    this.applyScale();
  }

  applyScale() {
    document.documentElement.style.fontSize = this.scale + 'px';
  }
}
