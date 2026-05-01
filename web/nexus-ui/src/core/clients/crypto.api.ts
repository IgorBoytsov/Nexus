import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PublicKeyResponse } from "../../contracts/responses/users/public-key-response";
import { HttpClient } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class CryptoApi {
    private http = inject(HttpClient);
    private baseUrl = 'http://127.0.0.1:5015';
    
    getPublicKey(): Observable<PublicKeyResponse> {
        return this.http.get<PublicKeyResponse>(`${this.baseUrl}/public-key`);
    }
}