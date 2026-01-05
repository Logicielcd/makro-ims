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

  getUser(id: string): Observable<User> {
    return this.apiService.get(`${this.apiController}/${id}`);
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

  deleteUser(userId: string) {
    return this.apiService.delete(
      `${this.apiController}/${userId}`
    );
  }

  resetPassword(user: UserRegister) {
    return this.apiService.post(`${this.apiController}/resetpassword`, user);
  }


  getTranslate(): Observable<Translate[]> {    
    return this.apiService.get(`${this.apiController}/translate`);    
  }
}
