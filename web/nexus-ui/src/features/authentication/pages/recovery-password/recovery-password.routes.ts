import { Routes } from "@angular/router";
import { guestGuard } from "../../../../core/guards/guest.guard";
import { recoveryPasswordStepGuard } from "./recovery-password-step.guard";

export const RECOVERY_PASSWORD_ROUTES: Routes = [
    {
        path: 'recovery/keys',
        loadComponent: () => import('./recovery-password-page.component').then(c => c.RecoveryPasswordPageComponent),
        canActivate: [guestGuard],
        children: [
            {
                path: '',
                loadComponent: () => import('./find-login/find-login.component').then(c => c.FindLoginComponent),
                canActivate: [recoveryPasswordStepGuard]
            },
            {
                path: 'code',
                loadComponent: () => import('./enter-code/enter-code.component').then(c => c.EnterCodeComponent),
                canActivate: [recoveryPasswordStepGuard]
            },
            {
                path: 'set',
                loadComponent: () => import('./set-password/set-password.component').then(c => c.SetPasswordComponent),
                canActivate: [recoveryPasswordStepGuard]
            },
            {
                path: '**',
                redirectTo: ''
            }
        ]
    }
]