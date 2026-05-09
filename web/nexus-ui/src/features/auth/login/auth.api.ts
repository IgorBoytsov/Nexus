import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { SrpChallengeRequest } from '../../../contracts/requests/srp-challenge.request'
import { SrpVerifyRequest } from '../../../contracts/requests/srp-verify.request'
import { SrpChallengeResponse } from '../../../contracts/responses/srp/srp-challenge.response'
import { AuthResponse } from '../../../contracts/responses/auth/auth.response'
import { Result } from '@crossdyne/toolkit';
import { ResultHttp } from '../../../core/result-helper/result-http';

@Injectable({
    providedIn: 'root'
})
export class AuthApi {
    private http: HttpClient = inject(HttpClient);

    getCrpChallenge(data: SrpChallengeRequest): Observable<Result<SrpChallengeResponse>> {
        return this.http.post<SrpChallengeResponse>(`/srp/challenge`, data)
        .pipe(
            map(response => ResultHttp.success<SrpChallengeResponse>(response)),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<SrpChallengeResponse>(error)))
        );
    }
    
    srpVerifyProof(data: SrpVerifyRequest): Observable<Result<AuthResponse>> {
        return this.http.post<AuthResponse>(`/srp/verify`, data, { withCredentials: true })
        .pipe(
            map(response => ResultHttp.success<AuthResponse>(response)),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<AuthResponse>(error)))
        );
    }
}