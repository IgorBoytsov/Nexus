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

    registerForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    constructor() {
        this.registerForm = this.fb.group({
            login: ['', [Validators.required, Validators.minLength(2)]],
            username: ['', [Validators.required, Validators.minLength(5)]],
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

            const ctx = await SrpContextFactory.create(SrpGroup.Rfc5054_3072);
            
            const crypto = new CryptoService();
            const keyDerivationService = new KeyDerivationService();
            const srp = new SrpClientService();
            const profile = CryptoProfileRegistry.latest;

            const { login, username, password, email } = this.registerForm.value;

            const publicKeyResponse = await firstValueFrom(this.register.getPublicKey());
            const firstParse = JSON.parse(publicKeyResponse.publicKey);
            const publicKeyBase64 = typeof firstParse === 'string' 
                ? firstParse 
                : firstParse.publicKey;

            const salt = crypto.generateRandomBytes(16);
            const saltBase64 = SecurityUtils.toBase64(salt);
            const { kek } = await keyDerivationService.deriveKeysFromPassword(login, password, salt);

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

            const srpAuthHashBytes = await keyDerivationService.deriveAuthHashForSrp(login, password, salt, ctx.hashAlgorithmName);
            const srpAuthHashBase64  =SecurityUtils.toBase64(srpAuthHashBytes);

            const verifierVase64 = await srp.generateSrpVerifier(srpAuthHashBase64, ctx);
            const verifierBytes = SecurityUtils.fromBase64(verifierVase64);

            const encryptedVerifierBuffer = await window.crypto.subtle.encrypt(
                { name: "RSA-OAEP" },
                rsaPublicKey,
                verifierBytes.buffer as ArrayBuffer
            );

            const encryptedVerifierBase64 = SecurityUtils.toBase64(new Uint8Array(encryptedVerifierBuffer));

            const dek = crypto.generateRandomBytes(32);
            const encryptedDek = await crypto.encryptData(dek, kek);
            
            const request: RegisterRequest = {
                login: login,
                userName: username,
                verifier: encryptedVerifierBase64,
                clientSalt: saltBase64,
                encryptedDek: encryptedDek,
                cryptoVersion: profile.version,
                email: email,
                idGender: null, 
                idCountry: null
            };
            
            const registerResult = await firstValueFrom(this.register.register(request));

            if (registerResult.isFailure){
                this.isLoading.set(false);
                this.errorMessage.set(registerResult.stringMessageFull);
            }

            console.log("Успешная регистрация!");

            this.isLoading.set(true);
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
            this.isLoading.set(true);
            this.errorMessage.set(null);

            this.router.navigate(['/login'])
        }
    }
}