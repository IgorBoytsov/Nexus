import { CanActivateChildFn, UrlTree } from "@angular/router";

export const recoveryStepGuard: CanActivateChildFn = (route): UrlTree | boolean => {
    return true;
}