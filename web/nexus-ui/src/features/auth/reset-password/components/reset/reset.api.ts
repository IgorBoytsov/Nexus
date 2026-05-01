import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { RecoveryPasswordRequest } from "../../../../../contracts/requests/recovery-password.request";

@Injectable({
    providedIn: 'root'
})
export class StepResetApi{
    private http = inject(HttpClient);
    private baseUrl = 'http://127.0.0.1:5015';
    
    recoveryAccessPassword = (data: RecoveryPasswordRequest) => 
        this.http.post<void>(`${this.baseUrl}/reset-password`, data);
}