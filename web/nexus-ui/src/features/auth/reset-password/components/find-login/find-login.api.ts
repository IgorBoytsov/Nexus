import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class StepLoginApi{
    private http = inject(HttpClient);

    generateCode = (login: string) => 
        this.http.post<void>(`/recovery-password/send-confirm-code/${login}`, null);
}