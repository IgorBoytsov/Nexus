import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { RegisterApi } from "./register.api";
import { RegisterRequest } from '../../../contracts/requests/register-user.request'
import { CryptoProfileRegistry, CryptoService, KeyDerivationService, SecurityUtils, SrpClientService, SrpContextFactory, SrpGroup } from "@crossdyne/security";
import { firstValueFrom } from "rxjs";
import { HttpErrorResponse } from "@angular/common/http";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
})

export class RegisterComponent{
    private fb = inject(FormBuilder);
    private router = inject(Router)
    private register = inject(RegisterApi)

    private readonly crypto = new CryptoService();
    private readonly keyDerivationService = new KeyDerivationService();
    private readonly srp = new SrpClientService();

    registerForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    readonly minLoginLength = 3;
    readonly minUsernameLength = 3;

    constructor() {
        this.registerForm = this.fb.group({
            login: ['', [Validators.required, Validators.minLength(this.minLoginLength)]],
            username: ['', [Validators.required, Validators.minLength(this.minUsernameLength)]],
            password: ['', [Validators.required, Validators.minLength(8)]],
            email: ['', [Validators.required, Validators.email]],
        });
    }

    async onSubmit(): Promise<void>{
        if (this.registerForm.invalid)
            return;
        
        this.isLoading.set(true);

        try {
            console.log("Началась регистрация!");
            
            //#region Конфигурация

            const srpGroup = SrpGroup.Rfc5054_3072;
            const ctx = await SrpContextFactory.create(srpGroup);
            
            const profile = CryptoProfileRegistry.latest;

            const { login, username, password, email } = this.registerForm.value;

            const salt = this.crypto.generateRandomBytes(16);

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

            const srpAuthHashBytes = await this.keyDerivationService.deriveAuthHashForSrp(login, password, salt, ctx.hashAlgorithmName);
            const srpAuthHashBase64 = SecurityUtils.toBase64(srpAuthHashBytes);

            const verifierBase64 = await this.srp.generateSrpVerifier(srpAuthHashBase64, ctx);

            const dekForVerifier = this.crypto.generateRandomBytes(32);
            const encryptedVerifier = await this.crypto.encryptData(verifierBase64, dekForVerifier, profile.aesGcmOptions);

            const encryptedKekForVerifier = await window.crypto.subtle.encrypt(
                { name: "RSA-OAEP" },
                rsaPublicKey,
                dekForVerifier.buffer as ArrayBuffer
            );

            const encryptedKekForVerifierBase64 = SecurityUtils.toBase64(new Uint8Array(encryptedKekForVerifier));

            //#endregion

            //#region Генерация DEK
           
            const saltBase64 = SecurityUtils.toBase64(salt);
            const { kek } = await this.keyDerivationService.deriveKeysFromPassword(login, password, salt);

            const dek = this.crypto.generateRandomBytes(32);
            const encryptedDek = await this.crypto.encryptData(dek, kek, profile.aesGcmOptions);

            //#endregion

            const request: RegisterRequest = {
                login: login,
                userName: username,
                verifier: encryptedVerifier,
                clientSalt: saltBase64,
                encryptedDek: encryptedDek,
                cryptoVersion: profile.version,
                srpVersion: srpGroup,
                encryptedVerifierWrapKey: encryptedKekForVerifierBase64,
                asymmetricKeyId: "env_v1",
                keyWrapVersion: profile.version, 
                email: email,
                idGender: null, 
                idCountry: null
            };
            
            const registerResult = await firstValueFrom(this.register.register(request));

            if (registerResult.isFailure){
                this.isLoading.set(false);
                this.errorMessage.set(registerResult.stringMessageFull);

                return;
            }

            console.log("Успешная регистрация!");

            this.errorMessage.set(null);

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