import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PublicKeyResponse } from "./responses/public-key-response";
import { HttpClient } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class CryptoApi {
    private http = inject(HttpClient);
    
    getPublicKey(): Observable<PublicKeyResponse> {
        return this.http.get<PublicKeyResponse>(`/public-key`);;
    }
}