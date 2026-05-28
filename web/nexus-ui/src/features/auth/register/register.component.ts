import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { RegisterApi } from "./register.api";
import { RegisterRequest } from '../../../contracts/requests/register-user.request'
import { CryptoProfileRegistry, CryptoService, CryptoVersion, KeyDerivationService, SecurityConstants, SecurityUtils, SrpClientService, SrpContextFactory, SrpGroup } from "@crossdyne/security";
import { firstValueFrom } from "rxjs";
import { HttpErrorResponse } from "@angular/common/http";
import { RecoveryKeysListComponent } from "../../../shared/ui/recovery-keys-list/recovery-keys-list.component";
import { CryptoConstants } from "../../../core/constants/security.constants";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, RecoveryKeysListComponent],
})

export class RegisterComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router)
    private register = inject(RegisterApi)

    private readonly crypto = new CryptoService();
    private readonly keyDerivationService = new KeyDerivationService();
    private readonly srp = new SrpClientService();

    registerForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);
    
    readonly showRecoveryKeys = signal(false);
    readonly generatedRecoveryKeys = signal<string[] | null>(null);

    readonly recoveryKeysDisplay: string[] = [];
    readonly recoveryAssets: Array<{encryptedDek: string, rowKey: Uint8Array, version: CryptoVersion}> = [];

    readonly minLoginLength = 3;
    readonly minUsernameLength = 3;
    readonly countRecoveryKays = CryptoConstants.RECOVERY_KEYS_COUNT; // 10

    constructor() {
        this.registerForm = this.fb.group({
            login: ['', [Validators.required, Validators.minLength(this.minLoginLength)]],
            username: ['', [Validators.required, Validators.minLength(this.minUsernameLength)]],
            password: ['', [Validators.required, Validators.minLength(8)]],
            email: ['', [Validators.required, Validators.email]],
        });
    }

    async onSubmit(): Promise<void> {
        if (this.registerForm.invalid)
            return;

        this.recoveryKeysDisplay.length = 0;
        this.recoveryAssets.length = 0;
        
        this.isLoading.set(true);

        try {
            console.log("Началась регистрация!");
            
            //#region Конфигурация

            const srpGroup = CryptoConstants.ACTUAL_SRP_GROUT; // Rfc5054_3072
            const ctx = await SrpContextFactory.create(srpGroup);
            
            const cryptoVersion = CryptoVersion.V1;
            const profile = CryptoProfileRegistry.getProfile(cryptoVersion);

            const { login, username, password, email } = this.registerForm.value;

            const srpAuthenticationSalt = this.crypto.generateRandomBytes(CryptoConstants.SALT_SIZE_BYTES); // 32
            const srpAuthenticationSaltBase64 = SecurityUtils.toBase64(srpAuthenticationSalt);

            const dekKeyDerivationSalt = this.crypto.generateRandomBytes(CryptoConstants.SALT_SIZE_BYTES); // 32
            const dekKeyDerivationSaltBase64 = SecurityUtils.toBase64(dekKeyDerivationSalt);
            //#endregion

            //#region Получение RSA ключа
            
            const publicKeyResponse = await firstValueFrom(this.register.getPublicKey());
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

            //#region Верификатор SRP

            const srpAuthHashBytes = await this.keyDerivationService.deriveAuthHashForSrp(login, password, srpAuthenticationSalt, ctx.hashAlgorithmName);
            const srpAuthHashBase64 = SecurityUtils.toBase64(srpAuthHashBytes);

            const verifierBase64 = await this.srp.generateSrpVerifier(srpAuthHashBase64, ctx);

            const dekForVerifier = this.crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32
            const encryptedVerifier = await this.crypto.encryptData(verifierBase64, dekForVerifier, profile.aesGcmOptions);

            const encryptedKekForVerifier = await window.crypto.subtle.encrypt(
                { name: "RSA-OAEP" },
                rsaPublicKey,
                dekForVerifier.buffer as ArrayBuffer
            );

            const encryptedKekForVerifierBase64 = SecurityUtils.toBase64(new Uint8Array(encryptedKekForVerifier));

            //#endregion

            //#region Генерация DEK

            const { kek } = await this.keyDerivationService.deriveKeysFromPassword(login, password, dekKeyDerivationSalt);

            const dek = this.crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32
            const encryptedDek = await this.crypto.encryptData(dek, kek, profile.aesGcmOptions);

            //#endregion

            //#region генерация ключей восстановления 

            for (let index = 0; index < this.countRecoveryKays; index++) {
                const rowKey = this.crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32
                const encryptedDek = await this.crypto.encryptData(dek, rowKey, profile.aesGcmOptions);
                this.recoveryKeysDisplay.push(SecurityUtils.toBase64(rowKey));
                this.recoveryAssets.push({encryptedDek: encryptedDek, rowKey: rowKey, version: cryptoVersion})
            }

            //#endregion

            //#region Отправка данных на сервер

            const request: RegisterRequest = {
                login: login,
                userName: username,
                encryptedVerifier: encryptedVerifier,
                srpSalt: srpAuthenticationSaltBase64,
                encryptedDek: encryptedDek,
                dekSalt: dekKeyDerivationSaltBase64,
                cryptoVersion: cryptoVersion,
                srpVersion: srpGroup,
                encryptedVerifierWrapKey: encryptedKekForVerifierBase64,
                asymmetricKeyId: "env_v1",
                keyWrapVersion: cryptoVersion, 
                email: email,
                idGender: null, 
                idCountry: null,
                recoveryKeys: this.recoveryAssets.map(a => ({encryptedValue: a.encryptedDek, cryptoVersion: a.version}))
            };
            
            const registerResult = await firstValueFrom(this.register.register(request));

            //#endregion

            if (registerResult.isFailure){
                this.isLoading.set(false);
                this.errorMessage.set(registerResult.stringMessageFull);

                return;
            }

            this.generatedRecoveryKeys.set(this.recoveryKeysDisplay);
            this.showRecoveryKeys.set(true);

            this.errorMessage.set(null);
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
            if (!this.showRecoveryKeys()){
                this.recoveryAssets.forEach(asset => asset.rowKey?.fill(0));
            }
        }
    }

    onRecoveryKeysConfirmed(): void {
        this.generatedRecoveryKeys.set(null);
        this.recoveryAssets.forEach(asset => asset.rowKey?.fill(0));

        this.router.navigate(['/login'])
    }
}