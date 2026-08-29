import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Result, Unit, UnitResult } from "@crossdyne/toolkit";
import { Observable, map, catchError, of } from "rxjs";
import { ResultHttp } from "../../../core/result-helper/result-http";
import { AuthResponse } from "../models/auth.response";
import { CompleteSrpRequest } from "../models/complete-srp.request";
import { SrpChallengeRequest } from "../models/srp-challenge.request";
import { SrpChallengeResponse } from "../models/srp-challenge.response";
import { SrpVerifyRequest } from "../models/srp-verify.request";
import { RegisterRequest } from "../models/register-user.request";
import { RecoveryViaKeysPayloadResponse } from "../models/recovery-via-keys-payload.response";
import { RecoveryViaKeysSetRequest } from "../models/recovery-via-keys-set.request";
import { ResetPasswordCompleteRequest } from "../models/reset-password-complete.request";

@Injectable({
    providedIn: 'root'
})
export class AuthenticationService {
    private http: HttpClient = inject(HttpClient);
    
    //#region Registration

    register(data: RegisterRequest): Observable<UnitResult> {
        return this.http.post('/register', data)
        .pipe(
            map(() => Result.success(Unit)), 
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }

    //#endregion

    //#region SRP Auth

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

    srpComplete(data: CompleteSrpRequest): Observable<Result<any>> {
        return this.http.post(`/srp/complete`, data, { withCredentials: true })
        .pipe(
            map(response => ResultHttp.success(response)),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure(error)))
        );
    }

    //#endregion

    //#region Recovery-Password
    
    existLogin(login: string) : Observable<Result<Unit>> {
        return this.http.get("/exist/user/login/", { params: { login: login } })
        .pipe(
            map(() => Result.success()),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    } 

    getRecoveryKeys(login: string): Observable<Result<RecoveryViaKeysPayloadResponse>> {
        return this.http.get<RecoveryViaKeysPayloadResponse>('/recovery/keys', { params: { login: login} })
        .pipe(
            map(response => Result.success<RecoveryViaKeysPayloadResponse>(response)),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<RecoveryViaKeysPayloadResponse>(error)))
        );
    }

    setPassword(request: RecoveryViaKeysSetRequest): Observable<Result<Unit>> {
        return this.http.post('/recovery/keys/password/change', request)
        .pipe(
            map(() => Result.success()),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }

    //#endregion

    //#region Reset-Password

    verifyConfirmCode(login: string, code: number): Observable<Result<Unit>>{
        return this.http.post(`/password/reset/confirm-code/${login}`, { code: code })
        .pipe(
            map(() => Result.success(Unit)),
            catchError(error => of(ResultHttp.failure<Unit>(error)))
        );
    }

    generateCode(login: string) : Observable<Result<Unit>> {
        return this.http.post(`/password/reset/send-confirm-code/${login}`, null)
        .pipe(
            map(() => Result.success()),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }

    recoveryAccessPassword(data: ResetPasswordCompleteRequest): Observable<Result<Unit>>{
        return this.http.post(`/password/reset`, data)
        .pipe(
            map(() => Result.success()),
            catchError(error =>  of(ResultHttp.failure<Unit>(error)))
        );
    } 

    //#endregion
}