import { BaseApiService } from "../../../shared/api/base.api.js";
import { RegisterRequest } from "../model/register-user.request.js";

export class RegistrationApi extends BaseApiService {
    constructor() {
        super("http://localhost:5017/api/users");
    }

    async register(data: RegisterRequest): Promise<void> {
        return await this.post<void>("", data);
    }
}