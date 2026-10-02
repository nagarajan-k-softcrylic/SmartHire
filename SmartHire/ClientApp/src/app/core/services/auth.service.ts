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

  /** Redirects to AuthBridge OIDC SSO challenge (backend). */
  ssoLogin(returnUrl: string = '/'): void {
    window.location.href = `/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  /** Legacy alias kept for backward compatibility with existing callers. */
  login(returnUrl: string = '/'): void {
    this.ssoLogin(returnUrl);
  }

  /** Registers a new local (username/password) account. */
  register(payload: { username: string; email: string; password: string }): Observable<{ message: string }> {
    return this.http.post<{ message: string }>('/auth/register', payload, { withCredentials: true });
  }

  /** Authenticates a local account and starts a cookie session. */
  localLogin(payload: { username: string; password: string }): Observable<{ message: string }> {
    this.currentUser$ = undefined;
    return this.http.post<{ message: string }>('/auth/local-login', payload, { withCredentials: true });
  }

  logout(): void {
    this.http.post('/auth/logout', {}, { withCredentials: true }).subscribe({
      complete: () => (window.location.href = '/'),
    });
  }
}
