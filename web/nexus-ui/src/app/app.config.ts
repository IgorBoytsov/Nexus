import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { bffBaseUrlInterceptor } from '../core/interceptors/bff-base-url.interceptor';
import { credentialsInterceptor } from '../core/interceptors/credentials.interceptor';
import { authErrorInterceptor } from '../core/interceptors/auth-error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([bffBaseUrlInterceptor, credentialsInterceptor, authErrorInterceptor])),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
  ]
};
