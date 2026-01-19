import { SecureDataService } from "../../../shared/lib/secure-data.service.js"
import { AuthApi } from "../api/auth.api.js";
import { UserApi } from "../../../entities/user/api/user.api.js";
import { SrpChallengeRequest } from "../model/srp-challenge.request.js";
import { SrpVerifyRequest } from "../model/srp-verify.request.js";

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
                const srpChallengeRequest: SrpChallengeRequest = { login: login };
                const challenge = await accountApiBFF.GetCrpChallenge(srpChallengeRequest);

                const srpProof = await crypto.generateSrpProof(password, challenge.salt, challenge.b);

                const srpVerifyRequest: SrpVerifyRequest = {
                    login: login,
                    a: srpProof.A,
                    m1: srpProof.M1
                };

                await accountApiBFF.SrpVerifyProof(srpVerifyRequest);

                window.location.href = '/Home/Privacy';

            } catch (error) {
                console.error("Ошибка в процессе входа:", error);
                alert(`Ошибка входа: ${error instanceof Error ? error.message : error}`);
            }
        });
    }
});