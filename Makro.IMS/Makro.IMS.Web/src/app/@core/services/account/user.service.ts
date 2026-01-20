import { Injectable } from '@angular/core';
import { Translate } from '@core/models/account/translate.model';
import { User } from '@core/models/account/user.model';
import { UserRegister } from '@core/models/auth/login-model';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiController = 'user';

  constructor(private apiService: ApiService) {}

  getAll(): Observable<User[]> {
    return this.apiService.get(`${this.apiController}`);
  }

  getUsers(): Observable<User[]> {
    return this.apiService.get(`${this.apiController}/user`);
  }

  getPendingUsers(): Observable<any[]> {
    return this.apiService.get(`${this.apiController}/pendinguser`);
  }

  getUser(id: string): Observable<User> {
    return this.apiService.get(`${this.apiController}/${id}`);
  }

  createUser(user: User): Observable<boolean> {    
    return this.apiService.post(`${this.apiController}/create`, user);
  }

  registerUser(user: UserRegister): Observable<boolean> {    
    return this.apiService.post(`${this.apiController}/login`, user);
  }

  updateUser(user: User) {    
    return this.apiService.put(`${this.apiController}`, user);
  }

  updateConfig(user: User) {
    return this.apiService.put(`${this.apiController}/config`, user);
  }

  deleteUser(user: User) {
    return this.apiService.post(
      `${this.apiController}/delete`,user
    );
  }

  resetPassword(user: UserRegister) {
    return this.apiService.post(`${this.apiController}/resetpassword`, user);
  }


  newPassword(user: User) {
    return this.apiService.post(`${this.apiController}/newpassword`, user);
  }

  getTranslate(): Observable<Translate[]> {    
    return this.apiService.get(`${this.apiController}/translate`);    
  }

  approvedUser(user: User) {
    return this.apiService.post(
      `${this.apiController}/approved`,user
    );
  }

  rejectUser(user: User) {
    return this.apiService.post(
      `${this.apiController}/reject`,user
    );
  }

}
