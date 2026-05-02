import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { RecoveryPasswordRequest } from "../../../../../contracts/requests/recovery-password.request";

@Injectable({
    providedIn: 'root'
})
export class StepResetApi{
    private http = inject(HttpClient);
    
    recoveryAccessPassword = (data: RecoveryPasswordRequest) => 
        this.http.post<void>(`/reset-password`, data);
}