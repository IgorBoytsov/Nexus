import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { StepEnterCodeApi } from "./enter-code.api";
import { firstValueFrom } from "rxjs";
import { RecoveryStateService } from "../../services/recovery-password-keys-state.service";
import { CryptoProfileRegistry, CryptoService, CryptoVersion, SecurityUtils } from "@crossdyne/security";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
    selector: 'app-enter-code',
    templateUrl: './enter-code.component.html',
    styleUrls: ['./enter-code.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule]
})
export class StepEnterCodeComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private http = inject(StepEnterCodeApi);
    private state = inject(RecoveryStateService)

    private readonly crypto = new CryptoService();

    stepEnterCodeForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    constructor(){
        this.stepEnterCodeForm = this.fb.group({
            code: ['', [Validators.required]]
        })
    }

    async onSubmit(): Promise<void> {
        if (this.stepEnterCodeForm.valid) {
            try {
                this.isLoading.set(true);
                this.errorMessage.set(null);

                const { code } = this.stepEnterCodeForm.value;
                const codeBytes = SecurityUtils.fromBase64(code.trim());

                const loginState = this.state.login;
                const result = await firstValueFrom(this.http.recoveryViaKeys({ login: loginState! }));

                if (result.isFailure) {
                    this.isLoading.set(false);
                    this.errorMessage.set(result.stringMessage);
                    return;
                }

                const { recoveryKeys } = result.value;
                let decryptedKeyStr: Uint8Array | null = null;
                let successfulCryptoVersion: number | null = null;

                for (const rk of recoveryKeys) {
                    const { key, cryptoVersion: rkCryptoVersion } = rk;
                    const profile = CryptoProfileRegistry.getProfile(rkCryptoVersion as CryptoVersion);

                    try {
                        decryptedKeyStr = await this.crypto.decryptData<Uint8Array>(key, codeBytes, profile.aesGcmOptions, true);
                        if (decryptedKeyStr) {
                            successfulCryptoVersion = rkCryptoVersion;
                            break;
                        }
                    } catch (error) {
                        console.warn('Ключ версии', rkCryptoVersion, 'не подошел');
                        continue;
                    }
                }

                if (!decryptedKeyStr) {
                    throw new Error('Не удалось расшифровать ни одним из ключей. Проверьте правильность кода восстановления.');
                }
   
                console.log('DEK успешно восстановлен');
                
                this.state.setDek(SecurityUtils.toBase64(decryptedKeyStr));
                this.router.navigate(['recovery/keys/set']);
                
            } catch (error) {
                console.error(`Произошла ошибка при восстановлении:`, error);
                this.errorMessage.set(error instanceof Error ? error.message : 'Произошла ошибка при восстановлении, попробуйте позже.');
            } finally {
                this.isLoading.set(false);
            }
        }
    }
}