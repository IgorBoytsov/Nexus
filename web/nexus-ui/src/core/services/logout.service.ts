import { Injectable } from "@angular/core";
import { Result } from "@crossdyne/toolkit";
import { HttpService } from "../http/http.service";

@Injectable({
    providedIn: 'root'
})
export class LogoutService extends HttpService {

    constructor(){
        super('');
    }

    async logout(): Promise<Result> {
        return await this.postAsync('/logout', null);
    }
}