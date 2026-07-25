import { Injectable } from "@angular/core";
import { ChangeEmailInitRequest } from "../models/change-email-init.request";
import { Result, Unit } from "@crossdyne/toolkit";
import { HttpService } from "../../../../../core/http/http.service";
import { ChangeEmailRequest } from "../models/change-email.request";

@Injectable({
    providedIn: 'root'
})
export class ChangeEmailService extends HttpService {

    constructor(){
        super('change/email');
    }

    async init(request: ChangeEmailInitRequest): Promise<Result<string>> {
        return await this.postAsync('/init', request);
    }
    
    async change(request: ChangeEmailRequest): Promise<Result<Unit>> {
        return await this.postAsync('', request);
    }
}