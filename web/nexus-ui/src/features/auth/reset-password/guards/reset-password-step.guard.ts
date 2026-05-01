import { inject } from "@angular/core";
import { CanActivateFn, Router, UrlTree } from "@angular/router";
import { RecoveryStateService } from "../services/reset-password-state.service";

export const recoveryStepGuard: CanActivateFn = (route): UrlTree | boolean => {
    const state = inject(RecoveryStateService);
    const router = inject(Router);
    const path = route.routeConfig?.path;

    if (path === 'code' && (!state.login || !state.isCodeSent))
        return router.createUrlTree(['/recovery']);

    if (path === 'reset' && !state.isCodeVerified)
        return router.createUrlTree(['/recovery/code']);

    return true;
}