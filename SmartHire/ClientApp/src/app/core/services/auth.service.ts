import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';

export interface CurrentUser {
  isAuthenticated: boolean;
  isAuthorized?: boolean;
  name?: string;
}

/**
 * Talks to the ASP.NET Core /auth endpoints backed by AuthBridge OIDC SSO.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUser$?: Observable<CurrentUser>;

  constructor(private readonly http: HttpClient) {}

  getCurrentUser(): Observable<CurrentUser> {
    if (!this.currentUser$) {
      this.currentUser$ = this.http
        .get<CurrentUser>('/auth/me', { withCredentials: true })
        .pipe(shareReplay(1));
    }
    return this.currentUser$;
  }

  login(returnUrl: string = '/'): void {
    window.location.href = `/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  logout(): void {
    this.http.post('/auth/logout', {}, { withCredentials: true }).subscribe({
      complete: () => (window.location.href = '/'),
    });
  }
}
