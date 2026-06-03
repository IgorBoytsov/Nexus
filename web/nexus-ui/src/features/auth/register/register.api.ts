import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, map, Observable, of } from "rxjs";
import { RegisterRequest } from "../../../contracts/requests/register-user.request";
import { Result, Unit, UnitResult } from "@crossdyne/toolkit";
import { ResultHttp } from "../../../core/result-helper/result-http";

@Injectable({
    providedIn: 'root'
})
export class RegisterApi {
    private http: HttpClient = inject(HttpClient);

    register(data: RegisterRequest): Observable<UnitResult> {
        return this.http.post('/register', data)
        .pipe(
            map(() => Result.success(Unit)), 
            catchError((error: HttpErrorResponse) => of(ResultHttp.failure<Unit>(error)))
        );
    }
}
