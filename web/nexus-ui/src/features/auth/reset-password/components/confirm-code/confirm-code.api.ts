import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class StepCodeApi{
    private http = inject(HttpClient);

    verifyConfirmCode = (login: string, code: number) => 
        this.http.post<void>(`/recovery-password/confirm-code/${login}/${code}`, null);
}