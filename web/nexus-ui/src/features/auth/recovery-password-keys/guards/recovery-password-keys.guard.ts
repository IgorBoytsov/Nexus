import { CanActivateFn, Router, UrlTree } from "@angular/router";
import { inject } from "@angular/core";
import { RecoveryStateService } from "../services/recovery-password-keys-state.service";

export const recoveryStepGuard: CanActivateFn = (route): UrlTree | boolean => {
    const recoveryState = inject(RecoveryStateService);
    const router = inject(Router);
    const path = route.routeConfig?.path;

    if (path === 'code' && !recoveryState.login) {
        return router.createUrlTree(['/recovery/keys']);
    }

    if (path === 'set' && !recoveryState.dek) {
        return router.createUrlTree(['/recovery/keys/code']);
    }

    return true;
}