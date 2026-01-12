import { BaseApiService } from "../../../shared/api/base.api.js";
import { LoginUserRequest } from "../model/login-user.request.js";

export class AuthApi extends BaseApiService {
    constructor() {
        super("http://localhost:5131/api/auth"); // BFF
    }

    async login(data: LoginUserRequest): Promise<void> {
        return await this.post<void>('/login', data);
    }
}