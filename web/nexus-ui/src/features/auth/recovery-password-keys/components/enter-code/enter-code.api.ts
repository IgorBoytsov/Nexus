import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Result } from "@crossdyne/toolkit";
import { catchError, map, Observable, of } from "rxjs";
import { RecoveryViaKeysPayloadResponse } from "../../../../../contracts/requests/recovery-via-keys-payload.response";
import { RecoveryViaKeysGetPayloadRequest } from "../../../../../contracts/requests/recovery-via-keys-get-payload.request";
import { ResultHttp } from "../../../../../core/result-helper/result-http";

@Injectable({
    providedIn: 'root'
})
export class StepEnterCodeApi {
    private http = inject(HttpClient);

    recoveryViaKeys(request: RecoveryViaKeysGetPayloadRequest): Observable<Result<RecoveryViaKeysPayloadResponse>> {
        return this.http.get<RecoveryViaKeysPayloadResponse>('/init-recovery-keys', { params: { login: request.login} })
        .pipe(
            map(response => Result.success<RecoveryViaKeysPayloadResponse>(response)),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<RecoveryViaKeysPayloadResponse>(error)))
        );
    }
}