import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { RecoveryStateService } from "../../services/reset-password-state.service";
import { CryptoProfileRegistry, CryptoService, CryptoVersion, SecurityUtils, SrpContextFactory } from "@crossdyne/security";
import { firstValueFrom } from "rxjs";
import { ResetPasswordCompleteRequest } from "../../../../../contracts/requests/reset-password-complete.request";
import { StepResetApi } from "./reset.api";
import { RecoveryKeysListComponent } from "../../../../../shared/ui/recovery-keys-list/recovery-keys-list.component";
import { CryptoConstants } from "../../../../../core/constants/security.constants";
import { RecoveryKeyService } from "../../../../../core/services/recovery-key.service";
import { ArrayUtils } from "../../../../../core/utils/array.utils";
import { RsaService } from "../../../../../core/services/rsa.service";
import { SrpVerifierService } from "../../../../../core/services/srp-verifier.service";
import { KeyManagementService } from "../../../../../core/services/key-management.service";

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
    private state = inject(RecoveryStateService);
    private stepResetApi = inject(StepResetApi);
    private recoveryKeyService = inject(RecoveryKeyService);
    private rsaService = inject(RsaService);
    private srpService = inject(SrpVerifierService);
    private keyManagement = inject(KeyManagementService);
    
    private readonly crypto = new CryptoService();

    resetForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    readonly countRecoveryKays = CryptoConstants.RECOVERY_KEYS_COUNT; // 10

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

            this.recoveryKeysDisplay.length = 0;
            this.recoveryAssets.length = 0;

            //#region Конфигурация

            const srpGroup = CryptoConstants.ACTUAL_SRP_GROUT; // Rfc5054_3072;
            const ctx = await SrpContextFactory.create(srpGroup);

            const cryptoVersion = CryptoVersion.V1;
            const profile = CryptoProfileRegistry.getProfile(cryptoVersion);

            const { newPassword } = this.resetForm.value;
            
            const srpAuthenticationSalt = this.crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32
            const srpAuthenticationSaltBase64 = SecurityUtils.toBase64(srpAuthenticationSalt);

            const dekKeyDerivationSalt = this.crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32
            const dekKeyDerivationSaltBase64 = SecurityUtils.toBase64(dekKeyDerivationSalt);

            //#endregion

            const rsaPublicKey = await this.rsaService.getPublicKey();

            const { encryptedVerifier, encryptedVerifierWrapKeyBase64 } = await this.srpService.generateVerifier(this.state.login!, newPassword, rsaPublicKey, srpAuthenticationSalt, ctx, profile);

            const { rawDek, encryptedDekBase64 } = await this.keyManagement.generateAndEncryptDek(this.state.login!, newPassword, dekKeyDerivationSalt, profile);

            const { recoveryKeysForDisplay, recoveryAssets } = await this.recoveryKeyService.generateKeys(this.crypto, rawDek, this.countRecoveryKays, profile);

            ArrayUtils.reset(this.recoveryKeysDisplay, recoveryKeysForDisplay);
            ArrayUtils.reset(this.recoveryAssets, recoveryAssets);

            const recoveryAccessPasswordRequest: ResetPasswordCompleteRequest = {
                login: this.state.login!,
                encryptedVerifier: encryptedVerifier,
                srpSalt: srpAuthenticationSaltBase64,
                srpVersion: srpGroup,
                encryptedVerifierWrapKey: encryptedVerifierWrapKeyBase64,
                keyWrapVersion: cryptoVersion, 
                asymmetricKeyId: "env_v1",
                encryptedDek: encryptedDekBase64,
                dekSalt: dekKeyDerivationSaltBase64,
                cryptoVersion: cryptoVersion,
                recoveryKeys: this.recoveryAssets.map(a => ({encryptedValue: a.encryptedDek, cryptoVersion: a.version}))
            }

            var recoveryResult = await firstValueFrom(this.stepResetApi.recoveryAccessPassword(recoveryAccessPasswordRequest));

            if (recoveryResult.isFailure){
                this.errorMessage.set(recoveryResult.stringMessage);
                return;
            }

            this.state.resetState();
            this.generatedRecoveryKeys.set(this.recoveryKeysDisplay);
            this.showRecoveryKeys.set(true);
        } catch (error) {
            console.error(error);
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