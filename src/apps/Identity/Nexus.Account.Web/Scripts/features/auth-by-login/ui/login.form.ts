import { SecureDataService } from "../../../shared/lib/secure-data.service.js"
import { AuthApi } from "../api/auth.api.js";
import { UserApi } from "../../../entities/user/api/user.api.js";
import { SrpChallengeRequest } from "../model/srp-challenge.request.js";
import { SrpVerifyRequest } from "../model/srp-verify.request.js";

const config = (window as any).AppConfig;

const crypto = new SecureDataService();
const accountApiBFF = new AuthApi(config.baseBFF);
const userApi = new UserApi(config.baseBFF);

document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.form') as HTMLFormElement;

    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const formData = new FormData(form);
            const login = formData.get('login') as string;
            const password = formData.get('password') as string;

            try {
                const srpChallengeRequest: SrpChallengeRequest = { login: login };
                const challenge = await accountApiBFF.GetCrpChallenge(srpChallengeRequest);

                const srpProof = await crypto.generateSrpProof(password, challenge.salt, challenge.b);

                const srpVerifyRequest: SrpVerifyRequest = {
                    Login: login,
                    A: srpProof.A,
                    M1: srpProof.M1
                };

                const response = await accountApiBFF.SrpVerifyProof(srpVerifyRequest) as any;
                const serverM2 = response.m2;

                const isServerValid = await crypto.verifyServerM2(srpProof.A, srpProof.M1, srpProof.S, serverM2);

                if (!isServerValid) {
                    throw new Error("Критическая ошибка: Подлинность сервера не подтверждена!");
                }

                window.location.href = '/Home/Privacy';

            } catch (error) {
                if (error instanceof Error && error.message.includes("400")) {
                    console.warn("Возможно токен безопасности устарен. Обновлям страницу...");
                    window.location.reload();
                }
                console.error("Ошибка в процессе входа:", error);
                alert(`Ошибка входа: ${error instanceof Error ? error.message : error}`);
            }
        });
    }
});