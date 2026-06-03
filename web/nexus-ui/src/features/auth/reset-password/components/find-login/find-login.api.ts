import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Result, Unit } from "@crossdyne/toolkit";
import { catchError, map, Observable, of } from "rxjs";
import { ResultHttp } from "../../../../../core/result-helper/result-http";

@Injectable({
    providedIn: 'root'
})
export class StepLoginApi{
    private http = inject(HttpClient);

    generateCode(login: string) : Observable<Result<Unit>> {
        return this.http.post(`/password/reset/send-confirm-code/${login}`, null)
        .pipe(
            map(() => Result.success()),
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }
        
}