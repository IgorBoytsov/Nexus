import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';

export const guestGuard: CanActivateFn = (route, state): Observable<boolean | UrlTree> => {
  const http = inject(HttpClient);
  const router = inject(Router);

  const isLogout = route.queryParamMap.get('logout') === 'true';

  if (isLogout) {
    const rawReturnUrl = route.queryParamMap.get('returnUrl') || '/';
    const isValidReturnUrl = rawReturnUrl.startsWith('/') && !rawReturnUrl.startsWith('//');
    const returnUrl = isValidReturnUrl ? rawReturnUrl : '/';
    
    const queryParams = returnUrl !== '/' ? { returnUrl } : {};
    const loginUrlTree = router.createUrlTree(['/login'], { queryParams });

    return http.post('/logout', null, { 
      withCredentials: true,
      headers: { 'X-Skip-Auth-Interceptor': 'true' }  
    }).pipe(
      map(() => loginUrlTree),
      catchError(() => of(loginUrlTree)) 
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