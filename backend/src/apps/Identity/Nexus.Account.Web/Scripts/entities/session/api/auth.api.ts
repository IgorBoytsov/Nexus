import { BaseApiService } from "../../../shared/api/base.api.js";
import { PublicKeyResponse } from "../model/public-key.response.js";

export class AuthApi extends BaseApiService {
    constructor(baseUrl: string) {
        super(baseUrl); // BFF
    }

    async refreshSession(): Promise<void> {
        return await this.post<void>('api/auth/login-by-token', {}); 
    }

    async getPublicKey(): Promise<PublicKeyResponse> {
        return await this.get<PublicKeyResponse>('api/auth/public-key');
    }
}