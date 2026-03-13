import { BaseApiService } from "../../../shared/api/base.api.js";
import { RegisterRequest } from "../model/register-user.request.js";

export class RegistrationApi extends BaseApiService {
    constructor(baseUrl: string) {
        super(baseUrl); // UserMenagement (Микро сервис)
    }

    async register(data: RegisterRequest): Promise<void> {
        return await this.post<void>("api/users", data);
    }
}