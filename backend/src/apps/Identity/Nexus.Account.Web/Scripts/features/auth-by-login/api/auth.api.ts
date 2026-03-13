import { BaseApiService } from "../../../shared/api/base.api.js";
import { LoginUserRequest } from "../model/login-user.request.js";
import { SrpChallengeRequest } from "../model/srp-challenge.request.js";
import { SrpChallengeResponse } from "../model/srp-challenge.response.js"; 
import { SrpVerifyRequest } from "../model/srp-verify.request.js";

export class AuthApi extends BaseApiService {
    constructor(baseUrl: string) {
        super(baseUrl); // BFF
    }

    async GetCrpChallenge(data: SrpChallengeRequest): Promise<SrpChallengeResponse> {
        return await this.post<SrpChallengeResponse>('api/auth/challenge', data);
    }

    async SrpVerifyProof(data: SrpVerifyRequest): Promise<any> {
        return await this.post<any>('api/auth/verify', data);
    }

    async login(data: LoginUserRequest): Promise<void> {
        return await this.post<void>('api/auth/login', data);
    }
}