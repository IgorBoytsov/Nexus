import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, of } from 'rxjs';

export const guestGuard: CanActivateFn = (route, state) => {
  const http = inject(HttpClient);
  const router = inject(Router);

  return http.get('/auth/status', { 
    withCredentials: true,
    headers: { 'X-Skip-Auth-Interceptor': 'true' }  
}).pipe(
    map(() => {
      return router.parseUrl('/user/profile');
    }),
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        return of(true);
      }
      
      return of(true); 
    })
  );
};