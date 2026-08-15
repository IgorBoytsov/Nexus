import { HttpInterceptorFn, HttpErrorResponse } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";

export const authErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  if (req.headers.has('X-Skip-Auth-Interceptor')) {
    return next(req);
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        router.navigate(['/login'], { 
          queryParams: { returnUrl: router.url } 
        });
      }
      
      return throwError(() => error);
    })
  );
};