import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, map, Observable, of } from "rxjs";
import { GetChangePasswordDataResponse } from "./models/get-change-password-data.response";
import { ChangePasswordRequest } from "./models/change-password.request";
import { Result, Unit } from "@crossdyne/toolkit";
import { ResultHttp } from "../../../core/result-helper/result-http";

@Injectable({
    providedIn: 'root'
})
export class ChangePasswordApi {
    private http = inject(HttpClient);

    init(): Observable<Result<GetChangePasswordDataResponse>> {
        return this.http.get<GetChangePasswordDataResponse>('/password/change/init', { withCredentials: true })
        .pipe(
            map(response => Result.success(response)), 
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<GetChangePasswordDataResponse>(error)))
        );
    }

    changePassword(request: ChangePasswordRequest): Observable<Result<Unit>> {
        return this.http.post<void>('/password/change', request, { withCredentials: true })
        .pipe(
            map(() => Result.success(Unit)), 
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }
}