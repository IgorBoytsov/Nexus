import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { ResetPasswordCompleteRequest } from "../../../../../contracts/requests/reset-password-complete.request";
import { catchError, map, Observable, of } from "rxjs";
import { ResultHttp } from "../../../../../core/result-helper/result-http";
import { Result, Unit } from "@crossdyne/toolkit";

@Injectable({
    providedIn: 'root'
})
export class StepResetApi{
    private http = inject(HttpClient);
    
    recoveryAccessPassword(data: ResetPasswordCompleteRequest): Observable<Result<Unit>>{
        return this.http.post(`/reset-password`, data)
        .pipe(
            map(() => Result.success()),
            catchError(error =>  of(ResultHttp.failure<Unit>(error)))
        );
    } 
        
}