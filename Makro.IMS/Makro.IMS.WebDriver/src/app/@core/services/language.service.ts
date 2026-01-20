import { Injectable } from '@angular/core';
import { AuthService } from 'auth/auth.service';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LanguageService {
  private language = new BehaviorSubject<string>(null);
  language$ = this.language.asObservable();

  constructor(private authService: AuthService) {
    const user = this.authService.getUser();
    this.setLanguage(user.translate);
  }

  setLanguage(language: string) {
    this.language.next(language);
  }
}
