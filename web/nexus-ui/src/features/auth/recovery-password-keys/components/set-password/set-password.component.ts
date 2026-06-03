import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { SetPasswordApi } from "./set-password.api";
import { RecoveryStateService } from "../../services/recovery-password-keys-state.service";
import { Router } from "@angular/router";
import { firstValueFrom } from "rxjs";
import { CryptoProfileRegistry, CryptoService, CryptoVersion, SecurityUtils, SrpContextFactory } from "@crossdyne/security";
import { RecoveryViaKeysSetRequest } from "../../../../../contracts/requests/recovery-via-keys-set.request";
import { RecoveryKeysListComponent } from "../../../../../shared/ui/recovery-keys-list/recovery-keys-list.component";
import { CryptoConstants } from "../../../../../core/constants/security.constants";
import { RecoveryKeyService } from "../../../../../core/services/recovery-key.service";
import { ArrayUtils } from "../../../../../core/utils/array.utils";
import { RsaService } from "../../../../../core/services/rsa.service";
import { SrpVerifierService } from "../../../../../core/services/srp-verifier.service";
import { KeyManagementService } from "../../../../../core/services/key-management.service";

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
    private setPasswordApi = inject(SetPasswordApi);
    private state = inject(RecoveryStateService);
    private recoveryKeyService = inject(RecoveryKeyService);
    private rsaService = inject(RsaService);
    private srpService = inject(SrpVerifierService);
    private keyManagement = inject(KeyManagementService);

    private readonly crypto = new CryptoService();

    readonly minLengthPassword = 9;
    readonly countRecoveryKays = CryptoConstants.RECOVERY_KEYS_COUNT; // 10

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

                this.recoveryKeysDisplay.length = 0;
                this.recoveryAssets.length = 0;

                //#region Конфигурация

                const { newPassword } = this.stepSetPasswordForm.value;

                const srpGroup = CryptoConstants.ACTUAL_SRP_GROUT; // Rfc5054_3072
                const ctx = await SrpContextFactory.create(srpGroup);

                const cryptoVersion = CryptoVersion.V1;
                const profile = CryptoProfileRegistry.getProfile(cryptoVersion);

                const srpAuthenticationSalt = this.crypto.generateRandomBytes(CryptoConstants.SALT_SIZE_BYTES); // 32
                const srpAuthenticationSaltBase64 = SecurityUtils.toBase64(srpAuthenticationSalt);

                const dekKeyDerivationSalt = this.crypto.generateRandomBytes(CryptoConstants.SALT_SIZE_BYTES); // 32
                const dekKeyDerivationSaltBase64 = SecurityUtils.toBase64(dekKeyDerivationSalt);

                //#endregion
                            
                const rsaPublicKey = await this.rsaService.getPublicKey();

                const { encryptedVerifier, encryptedVerifierWrapKeyBase64 } = await this.srpService.generateVerifier(this.state.login!, newPassword, rsaPublicKey, srpAuthenticationSalt, ctx, profile);

                const { rawDek, encryptedDekBase64 } = await this.keyManagement.reEncryptExistingDek(this.state.login!, newPassword, this.state.dek!, dekKeyDerivationSalt, profile);

                const { recoveryKeysForDisplay, recoveryAssets} = await this.recoveryKeyService.generateKeys(this.crypto, rawDek, this.countRecoveryKays, profile);

                ArrayUtils.reset(this.recoveryKeysDisplay, recoveryKeysForDisplay);
                ArrayUtils.reset(this.recoveryAssets, recoveryAssets);

                const request: RecoveryViaKeysSetRequest = {
                    login: this.state.login!,
                    encryptedVerifier: encryptedVerifier,
                    srpSalt: srpAuthenticationSaltBase64,
                    srpVersion: srpGroup,
                    encryptedVerifierWrapKey: encryptedVerifierWrapKeyBase64,
                    keyWrapVersion: cryptoVersion,
                    asymmetricKeyId: 'env_v1',
                    encryptedDek: encryptedDekBase64,
                    dekSalt: dekKeyDerivationSaltBase64,
                    cryptoVersion: cryptoVersion,
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
