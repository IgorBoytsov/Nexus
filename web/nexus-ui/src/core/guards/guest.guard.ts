import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';

export const guestGuard: CanActivateFn = (route, state): Observable<boolean | UrlTree> => {
  const http = inject(HttpClient);
  const router = inject(Router);

  const query = route.queryParamMap;

  if (query.get('logout') === 'true'){
    const returnUrl = query.get('returnUrl') || '/'; 
    
    return http.post('/logout', { 
      withCredentials: true,
       headers: { 'X-Skip-Auth-Interceptor': 'true' }  
      }).pipe(
        catchError(() => of(null)),
        map(() => {
          const params = returnUrl !== '/' ? { returnUrl } : {};
          return router.createUrlTree(['/login'], { queryParams: params });
        }),
      );
    }

    return http.get('/auth/status', {
      withCredentials: true,
      headers: { 'X-Skip-Auth-Interceptor': 'true' }
    }).pipe(
      map(() => router.parseUrl('/user/profile')),
      catchError(() => of(true))
    );
  };