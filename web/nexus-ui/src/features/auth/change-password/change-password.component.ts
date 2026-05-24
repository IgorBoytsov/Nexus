import { Component, inject, signal } from "@angular/core";
import { ChangePasswordApi } from "./change-password.api";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { firstValueFrom } from "rxjs";
import { CryptoProfileRegistry, CryptoService, CryptoVersion, KeyDerivationService, SecurityUtils, SrpClientService, SrpContextFactory, SrpGroup } from "@crossdyne/security";
import { CryptoApi } from "../../../core/clients/crypto.api";
import { ChangePasswordRequest } from "../../../contracts/requests/change-password.request";
import { HttpErrorResponse } from "@angular/common/http";

@Component({
    selector: 'profile-change-password',
    templateUrl: './change-password.component.html',
    styleUrls: ['./change-password.component..scss'],
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink]
})
export class ChangePasswordComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private changedPasswordApi = inject(ChangePasswordApi);
    private cryptoApi = inject(CryptoApi);

    private readonly cryptoService = new CryptoService();
    private readonly keyDerivationService = new KeyDerivationService();
    private readonly srpClientService = new SrpClientService();

    changePasswordForm: FormGroup; 
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    constructor() {
        this.changePasswordForm = this.fb.group({
            oldPassword: ['', Validators.required],
            newPassword: ['', Validators.required]
        })
    }

    async onSubmit(): Promise<void> {
        this.isLoading.set(true);

        try {
            const { oldPassword, newPassword } = this.changePasswordForm.value;
            const initResult = await firstValueFrom(this.changedPasswordApi.init());

            if (initResult.isFailure){
                this.errorMessage.set(initResult.stringMessage);
            }

            const { login, encryptedDek, cryptoVersionDek, clientSalt } = initResult.value;

            const profile = CryptoProfileRegistry.getProfile(cryptoVersionDek as CryptoVersion);

            const srgGroup = SrpGroup.Rfc5054_3072;
            const srpContext = await SrpContextFactory.create(srgGroup);

            const salt = await this.cryptoService.generateRandomBytes(16);
            const saltBase64 = SecurityUtils.toBase64(salt);

            //#region Получение RSA ключа
            
            const publicKeyResponse = await firstValueFrom(this.cryptoApi.getPublicKey());
            const firstParse = JSON.parse(publicKeyResponse.publicKey);
            const publicKeyBase64 = typeof firstParse === 'string' 
                ? firstParse 
                : firstParse.publicKey;
            const binaryKey = SecurityUtils.fromBase64(publicKeyBase64);
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

            //#endregion

            //#region Расшифровка - зашифровка DEK
            
            const clientSaltBytes = SecurityUtils.fromBase64(clientSalt);
            const { kek: oldKek } = await this.keyDerivationService.deriveKeysFromPassword(login, oldPassword, clientSaltBytes, profile.kdfOptions);
            const decryptedDek = await this.cryptoService.decryptData<string>(encryptedDek, oldKek, profile.aesGcmOptions);
            const { kek: newKek } = await this.keyDerivationService.deriveKeysFromPassword(login, newPassword, clientSaltBytes, profile.kdfOptions);
            const reEncryptedDek = await this.cryptoService.encryptData(decryptedDek, newKek, profile.aesGcmOptions);

            //#endregion

            //#region Верификатор SRP

            const srpAuthHashBytes = await this.keyDerivationService.deriveAuthHashForSrp(login, newPassword, salt, srpContext.hashAlgorithmName);
            const srpAuthHashBase64 = SecurityUtils.toBase64(srpAuthHashBytes);

            const dekForVerifier = this.cryptoService.generateRandomBytes(32);
            const verifierBase64 = await this.srpClientService.generateSrpVerifier(srpAuthHashBase64, srpContext);
            const encryptedVerifier = await this.cryptoService.encryptData(verifierBase64, dekForVerifier, profile.aesGcmOptions);

            const encryptedKeyForVerifier = await window.crypto.subtle.encrypt(
                { name: "RSA-OAEP" },
                rsaPublicKey,
                dekForVerifier.buffer as ArrayBuffer
            );

            const encryptedVerifierWrapKey = SecurityUtils.toBase64(new Uint8Array(encryptedKeyForVerifier))

            //#endregion

            //#region Отправка данных на сервер

            const request: ChangePasswordRequest = {
                userId: null,
                verifier: encryptedVerifier,
                clientSalt: saltBase64,
                encryptedDek: reEncryptedDek,
                cryptoVersion: profile.version,
                srpVersion: srgGroup,
                encryptedVerifierWrapKey: encryptedVerifierWrapKey,
                keyWrapVersion: profile.version,
                asymmetricKeyId: "env_v1"
            };
            
            await firstValueFrom(this.changedPasswordApi.changePassword(request));

            //#endregion

            this.router.navigate(['/login'])
        } catch (error) {
             console.error("Ошибка регистрации:", error);
            
            if (error instanceof HttpErrorResponse) {
                console.error('Status:', error.status);
                console.error('Error body:', error.error);
                console.error('Headers:', error.headers.keys());
            }

            this.errorMessage.set(error instanceof Error ? error.message : 'Неизвестная ошибка');
        } finally {
            this.isLoading.set(false);
        }
    }
}