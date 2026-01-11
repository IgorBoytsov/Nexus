import { SecureDataService } from "../../services/secure-data-service.js"
import { AccountApiService } from "../../services/account-api-service.js";
import { LoginUserRequest } from "../../Requests/login-user-request.js";
import { UserMenegementApiService, PublicEncryptionInfoResponse } from "../../auth/user-menagement-api-service.js";

const crypto = new SecureDataService();
const authApiClient = new AccountApiService();
const userApiClient = new UserMenegementApiService();

document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.form') as HTMLFormElement;

    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const formData = new FormData(form);
            const login = formData.get('login') as string;
            const password = formData.get('password') as string;

            try {

                const userPublicInfo = await userApiClient.getPublicEncryptionInfo(login);
                const saltBytes = crypto.fromBase64(userPublicInfo.clientSalt);

                const { kek, authHash } = await crypto.deriveKeysFromPassword(password, saltBytes); 

                const dek = await crypto.decryptData<Uint8Array>(userPublicInfo.encryptedDek, kek);

                const request: LoginUserRequest = {
                    Login: login,
                    Password: authHash
                };

                await authApiClient.login(request);

                window.location.href = '/Home/Privacy';

            } catch (error) {
                console.error("Ошибка в процессе входа:", error);
                alert(`Ошибка входа: ${error instanceof Error ? error.message : error}`);
            }
        });
    }
});