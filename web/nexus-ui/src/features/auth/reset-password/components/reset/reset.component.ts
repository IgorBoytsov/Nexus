import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { RecoveryStateService } from "../../services/reset-password-state.service";
import { CryptoProfileRegistry, CryptoService, KeyDerivationService, SecurityUtils, SrpService } from "@crossdyne/security";
import { CryptoApi } from "../../../../../core/clients/crypto.api";
import { firstValueFrom } from "rxjs";
import { RecoveryPasswordRequest } from "../../../../../contracts/requests/recovery-password.request";
import { StepResetApi } from "./reset.api";

@Component({
    selector: 'app-reset',
    templateUrl: './reset.component.html',
    styleUrls: ['./reset.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule]
})
export class StepResetComponent{
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);  
    private state = inject(RecoveryStateService);
    private cryptoApi = inject(CryptoApi);
    private stepResetApi = inject(StepResetApi);

    resetForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);
    
    constructor() {
        this.resetForm = this.fb.group({
            newPassword: ['', Validators.required]
        })
    }

    async onSubmit(): Promise<void> {
        try {
            this.isLoading.set(true);
            this.errorMessage.set(null);

            const crypto = new CryptoService();
            const keyDerivation = new KeyDerivationService();
            const srp = new SrpService();
            const profile = CryptoProfileRegistry.latest;

            const { newPassword } = this.resetForm.value;

            const publicKeyResponse = await firstValueFrom(this.cryptoApi.getPublicKey());
            const firstParse = JSON.parse(publicKeyResponse.publicKey);
            const publicKeyBase64 = typeof firstParse == 'string' ? firstParse :  firstParse.publicKey;

            const salt = crypto.generateRandomBytes(16);
            const saltBase64 = SecurityUtils.toBase64(salt);
            const { kek, authHash } = await keyDerivation.deriveKeysFromPassword(this.state.login!, newPassword, salt);
            
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

            const verifierBase64 = await srp.generateSrpVerifier(authHash);
            const verifierBytes = SecurityUtils.fromBase64(verifierBase64);

            const encryptedVerifierBuffer = await window.crypto.subtle.encrypt(
                { name: "RSA-OAEP" },
                rsaPublicKey,
                verifierBytes.buffer as ArrayBuffer
            );

            const encryptedVerifierBase64 = SecurityUtils.toBase64(new Uint8Array(encryptedVerifierBuffer));

            const dek = crypto.generateRandomBytes(32);
            const encryptedDek = await crypto.encryptData(dek, kek);

            const recoveryAccessPasswordRequest: RecoveryPasswordRequest = {
                Login: this.state.login!,
                Verifier: encryptedVerifierBase64,
                ClientSalt: saltBase64,
                EncryptedDek: encryptedDek,
                cryptoVersion: profile.version
            }

            var recoveryResult = await firstValueFrom(this.stepResetApi.recoveryAccessPassword(recoveryAccessPasswordRequest));

            if (recoveryResult.isFailure){
                this.errorMessage.set(recoveryResult.stringMessageFull);
                return;
            }

            alert("Пароль успешно изменен");

            this.state.resetState();
            this.router.navigate(['/login']);
        } catch (error) {
            console.error(error);
        } finally {
            this.isLoading.set(false);
        }

    }
}