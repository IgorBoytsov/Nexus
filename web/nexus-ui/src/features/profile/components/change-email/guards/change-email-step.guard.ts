import { inject } from "@angular/core";
import { CanActivateFn, Router, UrlTree } from "@angular/router";
import { ChangeEmailStateService } from "../services/change-email-state.service";

export const changeEmailStepGuard: CanActivateFn = (route): UrlTree | boolean => {
    const state = inject(ChangeEmailStateService);
    const router = inject(Router);
    const path = route.routeConfig?.path;

    if (path === 'confirm' && !state.email)
        return router.createUrlTree(['/change/email']);

    return true;
}