import { inject } from "@angular/core";
import { CanActivateFn, Router, UrlTree } from "@angular/router";
import { ResetPasswordStateService } from "../services/reset-password-state.service";

export const resetPasswordStepGuard: CanActivateFn = (route): UrlTree | boolean => {
    const state = inject(ResetPasswordStateService);
    const router = inject(Router);
    const path = route.routeConfig?.path;

    if (path === 'code' && (!state.login || !state.isCodeSent))
        return router.createUrlTree(['/reset']);

    if (path === 'reset' && !state.isCodeVerified)
        return router.createUrlTree(['/reset/code']);

    return true;
}