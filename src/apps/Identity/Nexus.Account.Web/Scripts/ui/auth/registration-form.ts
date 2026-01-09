import { SecureDataService } from "../../services/secure-data-service.js";
import { UserMenegementApiService, RegisterRequest } from "../../auth/user-menagement-api-service.js";

const crypto = new SecureDataService();
const apiClient = new UserMenegementApiService();

document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.form') as HTMLFormElement;

    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const sumbitBtn = form.querySelector('button') as HTMLButtonElement;
            const originalBtnText = sumbitBtn.innerText;
            sumbitBtn.disabled = true;
            sumbitBtn.innerText = "Регистрация...";

            const formData = new FormData(form);
            const password = formData.get('password') as string;
            const login = formData.get('login') as string;
            const username = formData.get('username') as string;
            const email = formData.get('email') as string;

            const phone = formData.get('phone') as string || null;
            const idGender = formData.get('idGender') as string || null;
            const idCountry = formData.get('idCountry') as string || null;

            try {
                const salt = crypto.generateRandomBytes(16);
                const saltBase64 = btoa(String.fromCharCode(...salt));
                const { kek, authHash } = await crypto.deriveKeysFromPassword(password, salt);
                const dek = crypto.generateRandomBytes(32);
                const encryptedDek = await crypto.encryptData(dek, kek);

                const request: RegisterRequest = {
                    Login: login,
                    UserName: username,
                    Password: authHash,
                    ClientSalt: saltBase64,
                    EncryptedDek: encryptedDek,
                    Email: email,
                    Phone: phone,
                    IdGender: idGender,
                    IdCountry: idCountry
                };

                await apiClient.register(request);

                alert("Регистрация прошла успешно!");

                kek.fill(0);

            } catch (error) {
                console.error("Ошибка в процессе регистрации:", error);
                alert(`Ошибка регистрации: ${error instanceof Error ? error.message : error}`);
            } finally {
                sumbitBtn.disabled = false;
                sumbitBtn.innerText = originalBtnText;
            }
        });
    }
});