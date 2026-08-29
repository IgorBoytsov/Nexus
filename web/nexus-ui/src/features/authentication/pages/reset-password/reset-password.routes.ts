import { Routes } from "@angular/router";
import { ResetPasswordPageComponent } from "./reset-password.page";
import { resetPasswordStepGuard } from "../../guards/reset-password-step.guard";

export const RESET_PASSWORD_ROUTES: Routes = [
    {
        path: 'reset',
        loadComponent: () => ResetPasswordPageComponent,
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