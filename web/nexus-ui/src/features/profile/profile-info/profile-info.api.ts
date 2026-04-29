import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ProfileInfo } from "./types";

@Injectable({
    providedIn: 'root'
})
export class ProfileInfoApi{
    private http: HttpClient = inject(HttpClient);
    private baseUrl = 'http://127.0.0.1:5015';

    getProfileInfo(): Observable<ProfileInfo> {
        return this.http.get<ProfileInfo>(`${this.baseUrl}/profile`, { withCredentials: true });
    }
}