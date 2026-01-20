import { Component,OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '@core/models/account/user.model';
import { LayoutService } from '@core/services/layout/layout.service';
import { AuthService } from 'auth/auth.service';
import { MenuItem } from 'primeng/api';
import { SignalRService } from '@core/services/signalr.service';
import { UserService } from '@core/services/account/user.service';
import { untilDestroyed } from '@ngneat/until-destroy';
import { NgForm } from '@angular/forms';
import { environment } from '@environments/environment';
import { SupplierService } from '@core/services/master/supplier.service';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
})
export class TopBarComponent {
  items!: MenuItem[];
  user: User;

  userRegisterVisible: boolean;
  newSupVisible: boolean;
  contactVisible: boolean;
  showNoti: boolean;
  showSubNoti: boolean;
  rejectVisible: boolean;
  showIcon: boolean;
  showIconSub: boolean;

  listUser: any[];
  listSups: any[];
  remark:string;

  selectedUser: any;

  htmlText: string;
  safeHtml: SafeHtml;
  
  videoUrl: string;
  videoVisible: boolean;

  menuItems$: Observable<MenuItem[]>;
  
  @ViewChild('f') f: NgForm;

  constructor(
    public layoutService: LayoutService,
    private authService: AuthService,
    public router: Router,
    public signalRService: SignalRService,
    private userService: UserService,
    private supplierService: SupplierService,
    private sanitizer: DomSanitizer,
    private http: HttpClient,
  ) {
    this.user = authService.getUser();
    this.menuItems$ = this.getMenuItems();

    if(this.user.userType == "ADMIN" || this.user.userType == "CONTROL"|| this.user.userType == "DEVELOP"){
      this.showNoti = true;
      this.showSubNoti = true;

      this.showIcon = true;
      this.showIconSub = true;
    }
    else{
      this.showNoti = false;
      this.showSubNoti = false;
      
      this.showIcon = false;
      this.showIconSub = false;
    }

    this.userRegisterVisible = false;
    this.rejectVisible = false;
    this.videoVisible = false;
    this.newSupVisible = false;
    // this.listUser = {} as any[];

  }

  ngOnInit():void{

    if(this.user.userType == "ADMIN" || this.user.userType == "CONTROL" || this.user.userType == "DEVELOP"){
      this.userService.getPendingUsers()
      .subscribe((a)=>{
        this.listUser = a;
      });

      this.supplierService.getNewSupplier()
      .subscribe((a)=>{
        this.listSups = a;
      });

      this.signalRService.startConnection();
      this.signalRService.hubConnection.on('userregister',(result:any)=>{
        this.listUser = result;        
      });
      
      this.signalRService.supplierHubConnection.on('supplier',(result:any)=>{
        this.listSups = result;
      });
    }
    else{
      this.showNoti = false;
      this.showSubNoti = false;
    }  
    
    this.getContactInfo();
  }

  getMenuItems(): Observable<MenuItem[]> {
    const menus: MenuItem[] = [      
      {
        label: 'Fresh',
        icon: 'pi pi-book',
        command: () => {
            this.loadManual();
        }
      },
      {
          label: 'Dry',
          icon: 'pi pi-book',          
          command: () => {
              this.loadManualDry();
          }
      },
      {
        label: 'Dry-Video',
        icon: 'pi pi-video',          
        command: () => {
            this.loadVideoDry();
        }
    },
    ];

    return of(menus);
  }

  getContactInfo(){   
              
    this.http.get('assets/files/contact.txt', { responseType: 'text' }).subscribe({
      next: (data) => {
        this.htmlText = data;        
        this.safeHtml = this.sanitizer.bypassSecurityTrustHtml(this.htmlText);
      },
      error: (error) => {
        //console.error('Error loading file:', error);
      }
    });  
      
  }

  userLogout() {
    this.authService.userLogout().subscribe((data) => {
      if (data) {
        this.router.navigate(['/auth/login']);
      }
    });
  }

  openUserRegisterDialog(){
    this.userRegisterVisible = true;
  }

  openContactDialog(){
    this.contactVisible = true;
  }

  openNewSupGroupDialog(){
    this.newSupVisible = true;
  }
  
  onApproved(data:any){
    this.selectedUser = data;
    let u: User = {} as User;
    u.userId = this.selectedUser.userId;
    u.userName = this.remark;
    this.userService.approvedUser(u).subscribe((a)=>{
      this.ngOnInit();
    });
  }

  onReject(data:any){
    this.selectedUser = data;
    this.rejectVisible = true;
  }

  onConfirmReject(){
    let u: User = {} as User;
    u.userId = this.selectedUser.userId;
    u.userName = this.remark;
    this.userService.rejectUser(u).subscribe((a)=>{
      this.rejectVisible = false;
      this.ngOnInit();
    });
  }

  loadManual(){    
    let link = document.createElement('a');
    link.setAttribute('type', 'hidden');
    link.href = `${environment.webUrl}` + '/assets/files/ims_sup_manual.pdf';
    link.download = 'ims_sup_manual.pdf';
    document.body.appendChild(link);
    link.click();
    link.remove();    
  }

  loadManualDry(){    
    let link = document.createElement('a');
    link.setAttribute('type', 'hidden');
    link.href = `${environment.webUrl}` + '/assets/files/ims_sup_manual_dry.pdf';
    link.download = 'ims_sup_manual_dry.pdf';
    document.body.appendChild(link);
    link.click();
    link.remove();    
  }
  
  loadVideoDry(){
    this.videoVisible = true;
    this.videoUrl = `${environment.webUrl}` + '/assets/files/vdo-dry.mp4';
  }

}
