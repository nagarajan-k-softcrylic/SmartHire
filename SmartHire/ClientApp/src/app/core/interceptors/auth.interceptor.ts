import { HttpInterceptorFn } from '@angular/common/http';

/**
 * Ensures cookies (the local SmartHire session established after AuthBridge OIDC sign-in)
 * are sent with every API request to the ASP.NET Core backend.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authReq = req.clone({ withCredentials: true });
  return next(authReq);
};
