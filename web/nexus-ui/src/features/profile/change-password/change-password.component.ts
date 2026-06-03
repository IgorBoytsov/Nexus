import { Component, inject, signal } from "@angular/core";
import { ChangePasswordApi } from "./change-password.api";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { firstValueFrom } from "rxjs";
import { CryptoProfileRegistry, CryptoService, CryptoVersion, KeyDerivationService, SecurityUtils, SrpContextFactory } from "@crossdyne/security";
import { ChangePasswordRequest } from "../../../contracts/requests/change-password.request";
import { HttpErrorResponse } from "@angular/common/http";
import { CryptoConstants } from "../../../core/constants/security.constants";
import { RsaService } from "../../../core/services/rsa.service";
import { SrpVerifierService } from "../../../core/services/srp-verifier.service";
import { KeyManagementService } from "../../../core/services/key-management.service";

@Component({
    selector: 'profile-change-password',
    templateUrl: './change-password.component.html',
    styleUrls: ['./change-password.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink]
})
export class ChangePasswordComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private changedPasswordApi = inject(ChangePasswordApi);
    private rsaService = inject(RsaService);
    private srpService = inject(SrpVerifierService);
    private keyManagement = inject(KeyManagementService);

    private readonly cryptoService = new CryptoService();
    private readonly keyDerivationService = new KeyDerivationService();

    changePasswordForm: FormGroup; 
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    constructor() {
        this.changePasswordForm = this.fb.group({
            oldPassword: ['', Validators.required],
            newPassword: ['', Validators.required]
        })
    }

    async onSubmit(): Promise<void> {
        this.isLoading.set(true);

        try {
            const { oldPassword, newPassword } = this.changePasswordForm.value;
            const initResult = await firstValueFrom(this.changedPasswordApi.init());

            if (initResult.isFailure){
                this.errorMessage.set(initResult.stringMessage);
                return;
            }

            //#region Конфигурация

            const { login, encryptedDek, cryptoVersionDek, dekSalt, srvVersion } = initResult.value;

            const profile = CryptoProfileRegistry.getProfile(cryptoVersionDek as CryptoVersion);

            const srpGroup = CryptoConstants.ACTUAL_SRP_GROUT;
            const ctx = await SrpContextFactory.create(srpGroup);

            const srpAuthenticationSalt = this.cryptoService.generateRandomBytes(CryptoConstants.SALT_SIZE_BYTES); // 32
            const srpAuthenticationSaltBase64 = SecurityUtils.toBase64(srpAuthenticationSalt);

            const dekKeyDerivationSalt = this.cryptoService.generateRandomBytes(CryptoConstants.SALT_SIZE_BYTES); // 32
            const dekKeyDerivationSaltBase64 = SecurityUtils.toBase64(dekKeyDerivationSalt);

            //#endregion

            const rsaPublicKey = await this.rsaService.getPublicKey();

            const { encryptedVerifier, encryptedVerifierWrapKeyBase64 } = await this.srpService.generateVerifier(login, newPassword, rsaPublicKey, srpAuthenticationSalt, ctx, profile);

            const reEncryptedDek = await this.keyManagement.reEncryptDekWithNewPassword(login, oldPassword, newPassword, dekSalt, encryptedDek, dekKeyDerivationSalt, profile);

            const request: ChangePasswordRequest = {
                userId: null,
                encryptedVerifier: encryptedVerifier,
                srpSalt: srpAuthenticationSaltBase64,
                srpVersion: srpGroup,
                encryptedVerifierWrapKey: encryptedVerifierWrapKeyBase64,
                keyWrapVersion: profile.version,
                asymmetricKeyId: "env_v1",
                encryptedDek: reEncryptedDek,
                dekSalt: dekKeyDerivationSaltBase64,
                cryptoVersion: profile.version,
            };
            
            await firstValueFrom(this.changedPasswordApi.changePassword(request));

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