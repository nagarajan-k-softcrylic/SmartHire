import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * Route guard enforcing claim-based authorization (app_access = RESUME_AI).
 * - Not authenticated -> redirect to the local /login page (username/password or SSO).
 * - Authenticated but missing claim -> redirect to /access-denied.
 */
export const authGuard: CanActivateFn = (_route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.getCurrentUser().pipe(
    map((user) => {
      if (!user.isAuthenticated) {
        return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
      }

      if (!user.isAuthorized) {
        return router.createUrlTree(['/access-denied']);
      }

      return true;
    }),
    catchError(() => of(router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } })))
  );
};
