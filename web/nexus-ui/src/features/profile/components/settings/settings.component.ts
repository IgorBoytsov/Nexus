import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { CryptoConfigurationService } from "../../../../core/services/crypto-configuration.service";
import { KeyManagementService } from "../../../../core/services/key-management.service";
import { SrpVerifierService } from "../../../../core/services/srp-verifier.service";
import { ChangePasswordService } from "../../services/change-password.service";
import { RsaService } from "../../../../core/services/rsa.service";
import { Router } from "@angular/router";
import { firstValueFrom } from "rxjs";
import { ChangePasswordRequest } from "../../models/change-password.request";
import { HttpErrorResponse } from "@angular/common/http";
import { CryptoVersion } from "@crossdyne/security";
import { CryptoConstants } from "../../../../core/constants/security.constants";

@Component({
    selector: 'settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule]
})
export class SettingsComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private changedPasswordApi = inject(ChangePasswordService);
    private rsaService = inject(RsaService);
    private srpService = inject(SrpVerifierService);
    private keyManagement = inject(KeyManagementService);
    private cryptoConfig = inject(CryptoConfigurationService);

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
            
            const oldCryptoVersion = cryptoVersionDek as CryptoVersion;
            const newCryptoVersion = this.cryptoConfig.getCryptoVersion();
            const srpGroup = await this.cryptoConfig.getSrpGroup(); // Rfc5054_3072

            const { rawSalt: rawSrpAuthSalt, saltBase64: base64SrpAuthSalt } = this.cryptoConfig.generateSalt(); // 32
            const { rawSalt: rawDekKeyDerivationSalt, saltBase64: base64DekKeyDerivationSalt } = this.cryptoConfig.generateSalt(); // 32

            //#endregion

            const rsaPublicKey = await this.rsaService.getPublicKey();

            const { encryptedVerifier, encryptedVerifierWrapKeyBase64 } = await this.srpService.generateVerifier(login, newPassword, rsaPublicKey, rawSrpAuthSalt, CryptoConstants.ACTUAL_SRP_GROUP, newCryptoVersion);

            const reEncryptedDek = await this.keyManagement.reEncryptDekWithNewPassword(login, oldPassword, newPassword, dekSalt, encryptedDek, rawDekKeyDerivationSalt, oldCryptoVersion, newCryptoVersion);

            const request: ChangePasswordRequest = {
                userId: null,
                encryptedVerifier: encryptedVerifier,
                srpSalt: base64SrpAuthSalt,
                srpVersion: srpGroup,
                srpCryptoVersion: newCryptoVersion,
                encryptedVerifierWrapKey: encryptedVerifierWrapKeyBase64,
                keyWrapVersion: newCryptoVersion,
                asymmetricKeyId: "env_v1",
                encryptedDek: reEncryptedDek,
                dekSalt: base64DekKeyDerivationSalt,
                cryptoVersion: newCryptoVersion,
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