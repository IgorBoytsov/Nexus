import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { ResultHttp } from "../../../../../core/result-helper/result-http";
import { Result, Unit } from "@crossdyne/toolkit";
import { catchError, map, Observable, of } from "rxjs";
import { ExistUserBuLoginRequest } from "../../../../../contracts/requests/exist-user-by-login.request";

@Injectable({
    providedIn: 'root'
})
export class StepLoginApi{
    private http = inject(HttpClient);

    existLogin(request: ExistUserBuLoginRequest) : Observable<Result<Unit>> {
        return this.http.post("/exist-user-by-login/", request)
        .pipe(
            map(() => Result.success()),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }     
}