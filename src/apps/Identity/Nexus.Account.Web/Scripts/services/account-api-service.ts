import { BaseApiService } from "../services/base-api-service.js";
import { LoginUserRequest } from "../Requests/login-user-request.js";

export class AccountApiService extends BaseApiService {
    constructor() {
        super("http://localhost:5131/api/auth");
    }

    async login(data: LoginUserRequest): Promise<void> {
        return await this.post<void>('/login', data);
    }
}