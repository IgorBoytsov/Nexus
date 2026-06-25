import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ProfileInfoResponse } from "./models/profile-info.response";

@Injectable({
    providedIn: 'root'
})
export class ProfileInfoApi{
    private http: HttpClient = inject(HttpClient);

    getProfileInfo(): Observable<ProfileInfoResponse> {
        return this.http.get<ProfileInfoResponse>(`/profile`, { withCredentials: true });
    }
}