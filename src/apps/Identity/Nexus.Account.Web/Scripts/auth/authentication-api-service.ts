import { BaseApiService } from "../services/base-api-service.js";
import { LoginUserRequest } from "../Requests/login-user-request.js";

export class AuthenticationApiService extends BaseApiService {
    constructor() {
        super("http://localhost:5093/api/auth");
    }

    async login(data: LoginUserRequest): Promise<void> {
        return await this.post<void>("/login", data);
    }
}