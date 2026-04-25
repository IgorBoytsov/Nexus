import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { PublicKeyResponse } from "../../../contracts/responses/users/public-key-response";
import { Observable } from "rxjs";
import { RegisterRequest } from "../../../contracts/requests/register-user.request";

@Injectable({
    providedIn: 'root'
})
export class RegisterApi {
    private http: HttpClient = inject(HttpClient);
    private baseUrl = 'http://127.0.0.1:5015';

    register(data: RegisterRequest): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/register`, data);
    }

    getPublicKey(): Observable<PublicKeyResponse> {
        return this.http.get<PublicKeyResponse>(`${this.baseUrl}/public-key`);
    }
}