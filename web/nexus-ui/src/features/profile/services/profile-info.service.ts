import { Injectable } from "@angular/core";
import { Result } from "@crossdyne/toolkit";
import { ProfileInfoResponse } from "../models/profile-info.response";
import { HttpService } from "../../../core/http/http.service";
import { ChangeUserNameRequest } from "../models/change-name.request";

@Injectable({
    providedIn: 'root'
})
export class ProfileInfoService extends HttpService {

    constructor() {
        super('');
    }

    async getProfileInfo(): Promise<Result<ProfileInfoResponse>> {
        return this.getAsync('/profile');
    }

    async changeAvatar(file: File): Promise<Result> {
        const formData = new FormData();

        formData.append('File', file);

        return await this.patchAsync('change/avatar', formData);
    }

    async changeName(request: ChangeUserNameRequest): Promise<Result> {
        return await this.patchAsync('change/name', request);
    }
}