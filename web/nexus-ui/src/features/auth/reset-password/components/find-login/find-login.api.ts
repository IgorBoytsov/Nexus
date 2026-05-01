import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class StepLoginApi{
    private http = inject(HttpClient);
    private baseUrl = 'http://127.0.0.1:5015';

    generateCode = (login: string) => 
        this.http.post<void>(`${this.baseUrl}/recovery-password/send-confirm-code/${login}`, null);
}