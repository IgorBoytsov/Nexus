import { BaseApiService } from "../../../shared/api/base.api.js";
import { PublicEncryptionInfoResponse } from "../model/public-encryption.response.js";

export class UserApi extends BaseApiService {
    constructor(baseUrl: string) {
        super(baseUrl); // BFF
    }

    async getPublicEncryptionInfo(login: string): Promise<PublicEncryptionInfoResponse> {
        const endpoint = `/public-encryption-info/${encodeURIComponent(login)}`;
        return await this.get<PublicEncryptionInfoResponse>(endpoint);
    }
}