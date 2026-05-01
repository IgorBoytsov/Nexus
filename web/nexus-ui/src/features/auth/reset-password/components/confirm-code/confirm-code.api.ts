import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class StepCodeApi{
    private http = inject(HttpClient);
    private baseUrl = 'http://127.0.0.1:5015';

    verifyConfirmCode = (login: string, code: number) => 
        this.http.post<void>(`${this.baseUrl}/recovery-password/confirm-code/${login}/${code}`, null);
}