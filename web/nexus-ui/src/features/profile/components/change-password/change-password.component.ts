import { Component, inject, signal } from "@angular/core";
import { ChangePasswordService } from "../../services/change-password.service";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { firstValueFrom } from "rxjs";
import { ChangePasswordRequest } from "../../models/change-password.request";
import { HttpErrorResponse } from "@angular/common/http";
import { RsaService } from "../../../../core/services/rsa.service";
import { SrpVerifierService } from "../../../../core/services/srp-verifier.service";
import { KeyManagementService } from "../../../../core/services/key-management.service";
import { CryptoConfigurationService } from "../../../../core/services/crypto-configuration.service";

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

            const { encryptedDek, cryptoVersionDek, dekSalt, srvVersion } = initResult.value;
            const login = initResult.value.login;
            const normalizeLogin = login.trim().toLowerCase();
            
            const { srpContext, srpGroup} = await this.cryptoConfig.getSrpContext(); // Rfc5054_3072
            const cryptoVersion = this.cryptoConfig.getCryptoVersion();

            const { rawSalt: rawSrpAuthSalt, saltBase64: base64SrpAuthSalt } = this.cryptoConfig.generateSalt(); // 32
            const { rawSalt: rawDekKeyDerivationSalt, saltBase64: base64DekKeyDerivationSalt } = this.cryptoConfig.generateSalt(); // 32

            //#endregion

            const rsaPublicKey = await this.rsaService.getPublicKey();

            const { encryptedVerifier, encryptedVerifierWrapKeyBase64 } = await this.srpService.generateVerifier(normalizeLogin, newPassword, rsaPublicKey, rawSrpAuthSalt, srpContext, cryptoVersion);

            const reEncryptedDek = await this.keyManagement.reEncryptDekWithNewPassword(normalizeLogin, oldPassword, newPassword, dekSalt, encryptedDek, rawDekKeyDerivationSalt, cryptoVersion);

            const request: ChangePasswordRequest = {
                userId: null,
                encryptedVerifier: encryptedVerifier,
                srpSalt: base64SrpAuthSalt,
                srpVersion: srpGroup,
                encryptedVerifierWrapKey: encryptedVerifierWrapKeyBase64,
                keyWrapVersion: cryptoVersion,
                asymmetricKeyId: "env_v1",
                encryptedDek: reEncryptedDek,
                dekSalt: base64DekKeyDerivationSalt,
                cryptoVersion: cryptoVersion,
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