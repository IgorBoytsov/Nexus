import { Routes } from "@angular/router";
import { AuthLayoutComponent } from "./layouts/auth-layout/auth-layout.component";
import { guestGuard } from "../../core/guards/guest.guard";
import { RECOVERY_PASSWORD_ROUTES } from "./pages/recovery-password/recovery-password.routes";
import { RESET_PASSWORD_ROUTES } from "./pages/reset-password/reset-password.routes";

export const AUTHENTICATION_ROUTES: Routes = [
    {
        path: '',
        component: AuthLayoutComponent,
        canActivate: [guestGuard],
        children: [
            {
                path: 'login',
                loadComponent: () => import('./pages/login/login-page.component').then(c => c.LoginPageComponent)
            },
            {
                path: 'register',
                loadComponent: () => import('./pages/register/register-page.component').then(c => c.RegisterPageComponent)
            },
            ...RECOVERY_PASSWORD_ROUTES,
            ...RESET_PASSWORD_ROUTES
        ]
    }
];