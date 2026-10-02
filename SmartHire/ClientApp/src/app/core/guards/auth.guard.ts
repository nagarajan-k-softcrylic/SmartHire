import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * Route guard enforcing claim-based authorization (app_access = RESUME_AI).
 * - Not authenticated -> redirect to /auth/login (AuthBridge OIDC challenge).
 * - Authenticated but missing claim -> redirect to /access-denied.
 */
export const authGuard: CanActivateFn = (_route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.getCurrentUser().pipe(
    map((user) => {
      if (!user.isAuthenticated) {
        window.location.href = `/auth/login?returnUrl=${encodeURIComponent(state.url)}`;
        return false;
      }

      if (!user.isAuthorized) {
        return router.createUrlTree(['/access-denied']);
      }

      return true;
    })
  );
};
