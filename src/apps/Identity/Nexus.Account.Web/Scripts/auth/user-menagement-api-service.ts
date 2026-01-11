import { BaseApiService } from "../services/base-api-service.js";

export interface RegisterRequest {
    Login: string,
    UserName: string,
    Password: string,
    ClientSalt: string,
    EncryptedDek: string,
    Email: string,
    Phone: string,
    IdGender: string,
    IdCountry: string
}

export interface PublicEncryptionInfoResponse {
    clientSalt: string,
    encryptedDek: string
}

export class UserMenegementApiService extends BaseApiService {
    constructor() {
        super("http://localhost:5017/api/users");
    }

    async getPublicEncryptionInfo(login: string): Promise<PublicEncryptionInfoResponse> {
        const endpoint = `/public-encryption-info/${encodeURIComponent(login)}`;
        return await this.get<PublicEncryptionInfoResponse>(endpoint);
    }

    async register(data: RegisterRequest): Promise<void> {
        return await this.post<void>("", data);
    }
}