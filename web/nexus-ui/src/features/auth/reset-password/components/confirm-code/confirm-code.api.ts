import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Result, Unit } from "@crossdyne/toolkit";
import { catchError, map, Observable, of } from "rxjs";
import { ResultHttp } from "../../../../../core/result-helper/result-http";

@Injectable({
    providedIn: 'root'
})
export class StepCodeApi{
    private http = inject(HttpClient);

    verifyConfirmCode(login: string, code: number): Observable<Result<Unit>>{
        return this.http.post(`/recovery-password/confirm-code/${login}`, { code: code })
        .pipe(
            map(() => Result.success(Unit)),
            catchError(error => of(ResultHttp.failure<Unit>(error)))
        );
    }
        
}