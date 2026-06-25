import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { RecoveryViaKeysSetRequest } from "./models/recovery-via-keys-set.request";
import { catchError, map, Observable, of } from "rxjs";
import { Result, Unit } from "@crossdyne/toolkit";
import { ResultHttp } from "../../../../../core/result-helper/result-http";

@Injectable({
    providedIn: 'root'
})
export class SetPasswordApi {
    private http = inject(HttpClient);

    setPassword(request: RecoveryViaKeysSetRequest): Observable<Result<Unit>> {
        return this.http.post('/recovery-via-keys', request)
        .pipe(
            map(() => Result.success()),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }
}