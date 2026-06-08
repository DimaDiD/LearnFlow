import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './services/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const user = authService.getCurrentUser();

  if (user?.accessToken) {
    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${user.accessToken}`
      }
    });
    return next(cloned);
  }

  return next(req);
};