import { Injectable, numberAttribute } from "@angular/core";

@Injectable({
    providedIn: 'root',
})
export class RecoveryStateService {
    login: string | null = null;
    dek: string | null = null;

    setLogin(login: string) {
        this.login = login
    }

    setDek(dek: string) {
        this.dek = dek;
    }

    reset() {
        this.login = null;
        this.dek = null;
    }
}