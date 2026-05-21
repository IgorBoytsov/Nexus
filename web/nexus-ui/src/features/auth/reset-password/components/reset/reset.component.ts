import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { RecoveryStateService } from "../../services/reset-password-state.service";
import { CryptoProfileRegistry, CryptoService, CryptoVersion, KeyDerivationService, SecurityUtils, SrpClientService, SrpContextFactory, SrpGroup } from "@crossdyne/security";
import { CryptoApi } from "../../../../../core/clients/crypto.api";
import { firstValueFrom } from "rxjs";
import { RecoveryPasswordRequest } from "../../../../../contracts/requests/recovery-password.request";
import { StepResetApi } from "./reset.api";
import { RecoveryKeysListComponent } from "../../../../../shared/ui/recovery-keys-list/recovery-keys-list.component";

@Component({
    selector: 'app-reset',
    templateUrl: './reset.component.html',
    styleUrls: ['./reset.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule, RecoveryKeysListComponent]
})
export class StepResetComponent{
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);  
    private state = inject(RecoveryStateService);
    private cryptoApi = inject(CryptoApi);
    private stepResetApi = inject(StepResetApi);
    private readonly crypto = new CryptoService();
    private readonly keyDerivation = new KeyDerivationService();
    private readonly srp = new SrpClientService();

    resetForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    readonly countRecoveryKays = 10;

    readonly showRecoveryKeys = signal(false);
    readonly generatedRecoveryKeys = signal<string[] | null>(null);

    readonly recoveryKeysDisplay: string[] = [];
    readonly recoveryAssets: Array<{encryptedDek: string, rowKey: Uint8Array, version: CryptoVersion}> = [];

    constructor() {
        this.resetForm = this.fb.group({
            newPassword: ['', [Validators.required, Validators.minLength(8)]]
        })
    }

    async onSubmit(): Promise<void> {
        try {
            this.isLoading.set(true);

            //#region Конфигурация

            const srpGroup = SrpGroup.Rfc5054_3072;
            const ctx = await SrpContextFactory.create(srpGroup);
            const profile = CryptoProfileRegistry.latest;

            const { newPassword } = this.resetForm.value;
            
            const salt = this.crypto.generateRandomBytes(16);

            //#endregion

            //#region Получение RSA ключа

            const publicKeyResponse = await firstValueFrom(this.cryptoApi.getPublicKey());
            const firstParse = JSON.parse(publicKeyResponse.publicKey);
            const publicKeyBase64 = typeof firstParse == 'string' ? firstParse :  firstParse.publicKey;

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

            const srpAuthHashBytes = await this.keyDerivation.deriveAuthHashForSrp(this.state.login!, newPassword, salt, ctx.hashAlgorithmName);
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
            const { kek } = await this.keyDerivation.deriveKeysFromPassword(this.state.login!, newPassword, salt, );

            const dek = this.crypto.generateRandomBytes(32);
            const encryptedDek = await this.crypto.encryptData(dek, kek, profile.aesGcmOptions);

            //#endregion

            //#region генерация ключей восстановления 

            for (let index = 0; index < this.countRecoveryKays; index++) {
                const rowKey = this.crypto.generateRandomBytes(32)
                const encryptedDek = await this.crypto.encryptData(dek, rowKey, profile.aesGcmOptions);
                this.recoveryKeysDisplay.push(SecurityUtils.toBase64(rowKey));
                this.recoveryAssets.push({encryptedDek: encryptedDek, rowKey: rowKey, version: profile.version})
            }

            //#endregion

            const recoveryAccessPasswordRequest: RecoveryPasswordRequest = {
                login: this.state.login!,
                verifier: encryptedVerifier,
                clientSalt: saltBase64,
                encryptedDek: encryptedDek,
                cryptoVersion: profile.version,
                srpVersion: srpGroup,
                encryptedVerifierWrapKey: encryptedKekForVerifierBase64,
                asymmetricKeyId: "env_v1",
                keyWrapVersion: profile.version, 
                recoveryKeys: this.recoveryAssets.map(a => ({encryptedValue: a.encryptedDek, cryptoVersion: a.version}))
            }

            var recoveryResult = await firstValueFrom(this.stepResetApi.recoveryAccessPassword(recoveryAccessPasswordRequest));

            if (recoveryResult.isFailure){
                this.errorMessage.set(recoveryResult.stringMessage);
                return;
            }

            alert("Пароль успешно изменен");

            this.state.resetState();
            this.generatedRecoveryKeys.set(this.recoveryKeysDisplay);
            this.showRecoveryKeys.set(true);
            this.errorMessage.set(null);
        } catch (error) {
            console.error(error);
            this.recoveryKeysDisplay.length = 0;
            this.recoveryAssets.length = 0;
        } finally {
            this.isLoading.set(false);
        }
    }
    
    onRecoveryKeysConfirmed(): void {
        this.generatedRecoveryKeys.set(null);
        this.recoveryAssets.forEach(asset => asset.rowKey?.fill(0));

        this.router.navigate(['/login'])
    }
}