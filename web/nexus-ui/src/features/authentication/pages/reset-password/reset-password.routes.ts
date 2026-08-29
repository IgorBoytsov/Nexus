import { Routes } from "@angular/router";
import { resetPasswordStepGuard } from "./reset-password-step.guard";

export const RESET_PASSWORD_ROUTES: Routes = [
    {
        path: 'reset',
        loadComponent: () => import('./reset-password-page.component').then(c => c.ResetPasswordPageComponent),
        children: [
            {
                path: '',
                loadComponent: () => import('./find-login/find-login.component').then(c => c.FindLoginComponent),
                canActivate: [resetPasswordStepGuard]
            },
            {
                path: 'code',
                loadComponent: () => import('./confirm-code/confirm-code.component').then(c => c.ConfirmCodeComponent),
                canActivate: [resetPasswordStepGuard]
            },
            {
                path: 'set',
                loadComponent: () => import('./reset-password/reset-password.component').then(c => c.ResetPasswordComponent),
                canActivate: [resetPasswordStepGuard]
            },
            {
                path: '**',
                redirectTo: ''
            }
        ]
    }
]