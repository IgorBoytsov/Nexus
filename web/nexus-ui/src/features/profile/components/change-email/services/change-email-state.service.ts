import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class ChangeEmailStateService {
    email: string | null = null;
    
    setEmail(newEmail: string) {
        this.email = newEmail;
    }

    resetState() {
        this.email = null;
    }
}