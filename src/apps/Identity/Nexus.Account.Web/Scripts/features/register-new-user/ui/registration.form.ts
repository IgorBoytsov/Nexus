import { SecureDataService } from "../../../shared/lib/secure-data.service.js";
import { RegistrationApi } from "../api/registration.api.js";
import { RegisterRequest } from "../model/register-user.request.js";
import { AuthApi } from "../../../entities/session/api/auth.api.js";

const crypto = new SecureDataService();
const registrationApiClient = new RegistrationApi();
const authApi = new AuthApi();


const hexToUint8Array = (hex: string): Uint8Array => {
    if (hex.length % 2 !== 0) hex = '0' + hex;
    const array = new Uint8Array(hex.length / 2);
    for (let i = 0; i < hex.length; i += 2) {
        array[i / 2] = parseInt(hex.substring(i, i + 2), 16);
    }
    return array;
};

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
                const publicKeyResponse = await authApi.getPublicKey();
                const publicKeyBase64 = (publicKeyResponse as any).publicKey || (publicKeyResponse as any).PublicKey;

                const salt = crypto.generateRandomBytes(16);
                const saltBase64 = btoa(String.fromCharCode(...salt));
                const { kek, authHash } = await crypto.deriveKeysFromPassword(password, salt);
                const verifierHex = await crypto.generateSrpVerifier(authHash);

                const binaryKey = crypto.fromBase64(publicKeyBase64);

                const rsaPublicKey = await window.crypto.subtle.importKey(
                    "spki",
                    binaryKey.buffer as ArrayBuffer,
                    {
                        name: "RSA-OAEP",
                        hash: "SHA-256"
                    },
                    false,
                    ["encrypt"]
                );

                const verifierBuffer = hexToUint8Array(verifierHex);

                const encryptedVerifierBuffer = await window.crypto.subtle.encrypt(
                    { name: "RSA-OAEP" },
                    rsaPublicKey,
                    verifierBuffer.buffer as ArrayBuffer
                );

                const encryptedVerifierBase64 = btoa(String.fromCharCode(...new Uint8Array(encryptedVerifierBuffer)));

                const dek = crypto.generateRandomBytes(32);
                const encryptedDek = await crypto.encryptData(dek, kek);

                const request: RegisterRequest = {
                    Login: login,
                    UserName: username,
                    Verifier: encryptedVerifierBase64,
                    ClientSalt: saltBase64,
                    EncryptedDek: encryptedDek,
                    Email: email,
                    Phone: phone,
                    IdGender: idGender,
                    IdCountry: idCountry
                };

                await registrationApiClient.register(request);

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