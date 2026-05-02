import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ProfileInfo } from "./types";

@Injectable({
    providedIn: 'root'
})
export class ProfileInfoApi{
    private http: HttpClient = inject(HttpClient);

    getProfileInfo(): Observable<ProfileInfo> {
        return this.http.get<ProfileInfo>(`/profile`, { withCredentials: true });
    }
}