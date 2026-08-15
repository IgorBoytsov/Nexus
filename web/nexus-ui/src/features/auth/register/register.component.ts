import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { RegisterApi } from "./register.api";
import { RegisterRequest } from './models/register-user.request'
import { CryptoService, CryptoVersion } from "@crossdyne/security";
import { firstValueFrom } from "rxjs";
import { HttpErrorResponse } from "@angular/common/http";
import { RecoveryKeysListComponent } from "../../../shared/ui/recovery-keys-list/recovery-keys-list.component";
import { CryptoConstants } from "../../../core/constants/security.constants";
import { RecoveryKeyService } from "../../../core/services/recovery-key.service";
import { ArrayHelper } from "../../../core/helpers/array.helper";
import { RsaService } from "../../../core/services/rsa.service";
import { SrpVerifierService } from "../../../core/services/srp-verifier.service";
import { KeyManagementService } from "../../../core/services/key-management.service";
import { CryptoConfigurationService } from "../../../core/services/crypto-configuration.service";

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
    private cryptoConfig = inject(CryptoConfigurationService);

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

            const srpGroup = await this.cryptoConfig.getSrpGroup(); // Rfc5054_3072
            const cryptoVersion = this.cryptoConfig.getCryptoVersion(); // V1

            const { rawSalt: rawSrpAuthSalt, saltBase64: base64SrpAuthSalt } = this.cryptoConfig.generateSalt(); // 32
            const { rawSalt: rawDekKeyDerivationSalt, saltBase64: base64DekKeyDerivationSalt } = this.cryptoConfig.generateSalt(); // 32

            const { username, password, email } = this.registerForm.value;

            const rawLogin = this.registerForm.value.login as string;
            const normalizeLogin = rawLogin.trim().toLowerCase();

            //#endregion

            const rsaPublicKey = await this.rsaService.getPublicKey();

            const { encryptedVerifier, encryptedVerifierWrapKeyBase64 } = await this.srpService.generateVerifier(normalizeLogin, password, rsaPublicKey, rawSrpAuthSalt, CryptoConstants.ACTUAL_SRP_GROUP, cryptoVersion);

            const { rawDek, encryptedDekBase64 } = await this.keyManagement.generateAndEncryptDek(normalizeLogin, password, rawDekKeyDerivationSalt, cryptoVersion);

            const { recoveryKeysForDisplay, recoveryAssets } = await this.recoveryKeyService.generateKeys(this.crypto, rawDek, this.countRecoveryKays, cryptoVersion);
            
            ArrayHelper.reset(this.recoveryKeysDisplay, recoveryKeysForDisplay);
            ArrayHelper.reset(this.recoveryAssets, recoveryAssets);

            const request: RegisterRequest = {
                login: normalizeLogin,
                userName: username,
                encryptedVerifier: encryptedVerifier,
                srpSalt: base64SrpAuthSalt,
                encryptedDek: encryptedDekBase64,
                dekSalt: base64DekKeyDerivationSalt,
                cryptoVersion: cryptoVersion,
                srpVersion: srpGroup,
                srpCryptoVersion: cryptoVersion,
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