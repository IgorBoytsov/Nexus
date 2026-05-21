import { Component, Inject, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { SetPasswordApi } from "./set-password.api";
import { RecoveryStateService } from "../../services/recovery-password-keys-state.service";
import { ActivatedRoute, Router } from "@angular/router";
import { CryptoApi } from "../../../../../core/clients/crypto.api";
import { firstValueFrom } from "rxjs";
import { CryptoProfileRegistry, CryptoService, CryptoVersion, KeyDerivationService, SecurityUtils, SrpClientService, SrpContextFactory, SrpGroup } from "@crossdyne/security";
import { RecoveryViaKeysSetRequest } from "../../../../../contracts/requests/recovery-via-keys-set.request";
import { RecoveryKeysListComponent } from "../../../../../shared/ui/recovery-keys-list/recovery-keys-list.component";

@Component({
    selector: 'app-set-password',
    templateUrl: './set-password.component.html',
    styleUrls: ['./set-password.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule, RecoveryKeysListComponent]
})
export class StepSetPasswordComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private setPasswordApi = inject(SetPasswordApi);
    private cryptoApi = inject(CryptoApi);
    private state = inject(RecoveryStateService);

    private readonly crypto = new CryptoService();
    private readonly keyDerivation = new KeyDerivationService();
    private readonly srp = new SrpClientService();

    readonly minLengthPassword = 9;
    readonly countRecoveryKays = 10;

    stepSetPasswordForm: FormGroup;
    errorMessage = signal<string | null>(null);
    isLoading = signal(false);

    readonly showRecoveryKeys = signal(false);
    readonly generatedRecoveryKeys = signal<string[] | null>(null);

    readonly recoveryKeysDisplay: string[] = [];
    readonly recoveryAssets: Array<{encryptedDek: string, rowKey: Uint8Array, version: CryptoVersion}> = [];

    constructor(){
        this.stepSetPasswordForm = this.fb.group({
            newPassword: ['', [Validators.required, Validators.minLength(this.minLengthPassword)]]
        })
    }

    async onSubmit(): Promise<void> {
        if (this.stepSetPasswordForm.valid){
            try {
                this.isLoading.set(true);

                //#region Конфигурация

                const { newPassword } = this.stepSetPasswordForm.value;
                const srpGroup = SrpGroup.Rfc5054_3072;
                const ctx = await SrpContextFactory.create(srpGroup);
                const profile = CryptoProfileRegistry.latest;
                const salt = this.crypto.generateRandomBytes(16);
                const saltBase64 = SecurityUtils.toBase64(salt);

                //#endregion
                
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

                //#region Перешифрование DEK
                
                const { kek } = await this.keyDerivation.deriveKeysFromPassword(this.state.login!, newPassword, salt);
                const encryptedDek = await this.crypto.encryptData(this.state.dek!, kek, profile.aesGcmOptions);

                //#endregion

                //#region генерация ключей восстановления 

                for (let index = 0; index < this.countRecoveryKays; index++) {
                    const rowKey = this.crypto.generateRandomBytes(32)
                    const encryptedDek = await this.crypto.encryptData(this.state.dek, rowKey, profile.aesGcmOptions);
                    this.recoveryKeysDisplay.push(SecurityUtils.toBase64(rowKey));
                    this.recoveryAssets.push({encryptedDek: encryptedDek, rowKey: rowKey, version: profile.version})
                }

                //#endregion

                const request: RecoveryViaKeysSetRequest = {
                    login: this.state.login!,
                    verifier: encryptedVerifier,
                    clientSalt: saltBase64,
                    encryptedVerifierWrapKey: encryptedKekForVerifierBase64,
                    cryptoVersion: profile.version,
                    srpVersion: srpGroup,
                    encryptedDek: encryptedDek,
                    keyWrapVersion: profile.version,
                    asymmetricKeyId: 'env_v1',
                    recoveryKeys: this.recoveryAssets.map(a => ({ encryptedValue: a.encryptedDek, cryptoVersion: a.version }))
                }

                const setPasswordResult = await firstValueFrom(this.setPasswordApi.setPassword(request));

                if (setPasswordResult.isFailure){
                    this.isLoading.set(false);
                    this.errorMessage.set(setPasswordResult.stringMessageFull);
                    return;
                }

                this.state.reset();
                this.generatedRecoveryKeys.set(this.recoveryKeysDisplay);
                this.showRecoveryKeys.set(true);
                this.errorMessage.set(null);
            } catch (error) {
                console.error('Ошибка: ', error);
                this.recoveryKeysDisplay.length = 0;
                this.recoveryAssets.length = 0;
            } finally {
                 this.isLoading.set(false);
            }
        }
    }

    onRecoveryKeysConfirmed(): void {
        this.generatedRecoveryKeys.set(null);
        this.recoveryAssets.forEach(asset => asset.rowKey?.fill(0));
        this.router.navigate(['/login'])
    }
}
