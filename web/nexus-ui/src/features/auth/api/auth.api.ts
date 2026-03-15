import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SrpChallengeRequest } from '../../../contracts/requests/srp-challenge.request'
import { SrpVerifyRequest } from '../../../contracts/requests/srp-verify.request'
import { SrpChallengeResponse } from '../../../contracts/responses/srp/srp-challenge.response'
import { AuthResponse } from '../../../contracts/responses/auth/auth.response'

@Injectable({
    providedIn: 'root'
})
export class AuthApi {
    private http: HttpClient = inject(HttpClient);
    private baseUrl = 'http://localhost:5015';

    getCrpChallenge(data: SrpChallengeRequest): Observable<SrpChallengeResponse> {
        return this.http.post<SrpChallengeResponse>(`${this.baseUrl}/srp/challenge`, data);
    }
    
    srpVerifyProof(data: SrpVerifyRequest): Observable<AuthResponse> {
        return this.http.post<AuthResponse>(`${this.baseUrl}/srp/verify`, data, { withCredentials: true });
    }
}