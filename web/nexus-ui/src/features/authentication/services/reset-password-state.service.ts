import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class ResetPasswordStateService{
    login: string | null = null;
    isCodeSent = false;
    isCodeVerified = false;

    setLogin(login: string){
        this.login = login
        this.isCodeSent = true;
    }

    verifyCode(code: string) : boolean {
        this.isCodeVerified = true;
        return true;
    }

    resetState() {
        this.login = null;
        this.isCodeSent = false;
        this.isCodeVerified = false;
    }
}