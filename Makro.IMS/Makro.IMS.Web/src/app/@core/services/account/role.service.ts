import { Injectable } from '@angular/core';
import { Role } from '@core/models/account/role.model';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private apiController = 'role';

  constructor(private apiService: ApiService) {}

  getRoles(): Observable<Role[]> {
    return this.apiService.get(`${this.apiController}`);
  }

  getRole(roleName: string): Observable<Role> {
    return this.apiService.get(`${this.apiController}/${roleName}`);
  }

  createRole(role: Role): Observable<Role> {
    return this.apiService.post(`${this.apiController}`, role);
  }

  updateRole(role: Role): Observable<Role> {
    return this.apiService.put(`${this.apiController}`, role);
  }

  deleteRole(roleName: string): Observable<Role> {
    return this.apiService.delete(`${this.apiController}/${roleName}`);
  }
}
