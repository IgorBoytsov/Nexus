import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { RegisterApi } from "./register.api";
import { RegisterRequest } from '../../../contracts/requests/register-user.request'
import { CryptoProfileRegistry, CryptoService, CryptoVersion, SecurityUtils, SrpContextFactory } from "@crossdyne/security";
import { firstValueFrom } from "rxjs";
import { HttpErrorResponse } from "@angular/common/http";
import { RecoveryKeysListComponent } from "../../../shared/ui/recovery-keys-list/recovery-keys-list.component";
import { CryptoConstants } from "../../../core/constants/security.constants";
import { RecoveryKeyService } from "../../../core/services/recovery-key.service";
import { ArrayUtils } from "../../../core/utils/array.utils";
import { RsaService } from "../../../core/services/rsa.service";
import { SrpVerifierService } from "../../../core/services/srp-verifier.service";
import { KeyManagementService } from "../../../core/services/key-management.service";

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
    private recoveryKeyService = inject(RecoveryKeyService);
    private rsaService = inject(RsaService);
    private srpService = inject(SrpVerifierService);
    private keyManagement = inject(KeyManagementService);

    private readonly crypto = new CryptoService();

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

            const rsaPublicKey = await this.rsaService.getPublicKey();

            const { encryptedVerifier, encryptedVerifierWrapKeyBase64 } = await this.srpService.generateVerifier(login, password, rsaPublicKey, srpAuthenticationSalt, ctx, profile);

            const { rawDek, encryptedDekBase64 } = await this.keyManagement.generateAndEncryptDek(login, password, dekKeyDerivationSalt, profile);

            const { recoveryKeysForDisplay, recoveryAssets } = await this.recoveryKeyService.generateKeys(this.crypto, rawDek, this.countRecoveryKays, profile);
            
            ArrayUtils.reset(this.recoveryKeysDisplay, recoveryKeysForDisplay);
            ArrayUtils.reset(this.recoveryAssets, recoveryAssets);

            const request: RegisterRequest = {
                login: login,
                userName: username,
                encryptedVerifier: encryptedVerifier,
                srpSalt: srpAuthenticationSaltBase64,
                encryptedDek: encryptedDekBase64,
                dekSalt: dekKeyDerivationSaltBase64,
                cryptoVersion: cryptoVersion,
                srpVersion: srpGroup,
                encryptedVerifierWrapKey: encryptedVerifierWrapKeyBase64,
                asymmetricKeyId: "env_v1",
                keyWrapVersion: cryptoVersion, 
                email: email,
                idGender: null, 
                idCountry: null,
                recoveryKeys: this.recoveryAssets.map(a => ({encryptedValue: a.encryptedDek, cryptoVersion: a.version}))
            };
            
            const registerResult = await firstValueFrom(this.register.register(request));

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