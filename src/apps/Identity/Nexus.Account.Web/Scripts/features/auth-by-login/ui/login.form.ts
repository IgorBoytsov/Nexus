import { SecureDataService } from "../../../shared/lib/secure-data.service.js"
import { AuthApi } from "../api/auth.api.js";
import { LoginUserRequest } from "../model/login-user.request.js";
import { UserApi } from "../../../entities/user/api/user.api.js";

const crypto = new SecureDataService();
const accountApiBFF = new AuthApi();
const userApi = new UserApi();

document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.form') as HTMLFormElement;

    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const formData = new FormData(form);
            const login = formData.get('login') as string;
            const password = formData.get('password') as string;

            try {

                const userPublicInfo = await userApi.getPublicEncryptionInfo(login);
                const saltBytes = crypto.fromBase64(userPublicInfo.clientSalt);

                const { kek, authHash } = await crypto.deriveKeysFromPassword(password, saltBytes); 

                const dek = await crypto.decryptData<Uint8Array>(userPublicInfo.encryptedDek, kek);

                const request: LoginUserRequest = {
                    Login: login,
                    Password: authHash
                };

                await accountApiBFF.login(request);

                window.location.href = '/Home/Privacy';

            } catch (error) {
                console.error("Ошибка в процессе входа:", error);
                alert(`Ошибка входа: ${error instanceof Error ? error.message : error}`);
            }
        });
    }
});