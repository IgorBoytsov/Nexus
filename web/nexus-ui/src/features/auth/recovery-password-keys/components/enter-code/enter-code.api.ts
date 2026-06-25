import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Result } from "@crossdyne/toolkit";
import { catchError, map, Observable, of } from "rxjs";
import { ResultHttp } from "../../../../../core/result-helper/result-http";
import { RecoveryViaKeysPayloadResponse } from "./models/recovery-via-keys-payload.response";

@Injectable({
    providedIn: 'root'
})
export class StepEnterCodeApi {
    private http = inject(HttpClient);

    getRecoveryKeys(login: string): Observable<Result<RecoveryViaKeysPayloadResponse>> {
        return this.http.get<RecoveryViaKeysPayloadResponse>('/recovery-keys', { params: { login: login} })
        .pipe(
            map(response => Result.success<RecoveryViaKeysPayloadResponse>(response)),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<RecoveryViaKeysPayloadResponse>(error)))
        );
    }
}