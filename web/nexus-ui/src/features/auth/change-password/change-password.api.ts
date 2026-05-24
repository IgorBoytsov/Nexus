import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, map, Observable, of } from "rxjs";
import { ChangePasswordInitResponse } from "../../../contracts/responses/auth/change-password-init.response";
import { ChangePasswordRequest } from "../../../contracts/requests/change-password.request";
import { Result, Unit } from "@crossdyne/toolkit";
import { ResultHttp } from "../../../core/result-helper/result-http";

@Injectable({
    providedIn: 'root'
})
export class ChangePasswordApi {
    private http = inject(HttpClient);

    init(): Observable<Result<ChangePasswordInitResponse>> {
        return this.http.post<ChangePasswordInitResponse>('/change-password-init', null, { withCredentials: true })
        .pipe(
            map(response => Result.success(response)), 
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<ChangePasswordInitResponse>(error)))
        );
    }

    changePassword(request: ChangePasswordRequest): Observable<Result<Unit>> {
        return this.http.post<void>('/change-password', request, { withCredentials: true })
        .pipe(
            map(() => Result.success(Unit)), 
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }
}