import { BaseApiService } from "../../../shared/api/base.api.js";

export class AuthApi extends BaseApiService {
    constructor() {
        super("http://localhost:5131/api/auth"); // BFF
    }

    async refreshSession(): Promise<void> {
        return await this.post<void>('/login-by-token', {}); 
    }
}