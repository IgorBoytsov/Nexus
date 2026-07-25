import { Injectable } from "@angular/core";
import { HttpService } from "../../../core/http/http.service";
import { Result } from "@crossdyne/toolkit";

@Injectable({
    providedIn: 'root'
})
export class ManagementAccountService extends HttpService {
    constructor() {
        super('')
    }

    async deleteAccountAsync() : Promise<Result> {
        return await this.deleteAsync('account/delete');
    }
}