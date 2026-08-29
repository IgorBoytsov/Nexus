import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { firstValueFrom } from "rxjs";
import { CryptoService, SecurityUtils } from "@crossdyne/security";
import { ActivatedRoute, Router } from "@angular/router";
import { RecoveryPasswordStateService } from "../../../services/recovery-password-state.service";
import { AuthenticationService } from "../../../services/authentication.service";

@Component({
    selector: 'app-enter-code',
    templateUrl: './enter-code.component.html',
    styleUrls: ['./enter-code.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule]
})
export class EnterCodeComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private http = inject(AuthenticationService);
    private state = inject(RecoveryPasswordStateService)

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
                const result = await firstValueFrom(this.http.getRecoveryKeys(loginState!));

                if (result.isFailure) {
                    this.isLoading.set(false);
                    this.errorMessage.set(result.stringMessage);
                    return;
                }

                const { recoveryKeys } = result.value;
                let decryptedKeyStr: Uint8Array | null = null;

                for (const rk of recoveryKeys) {
                    const { key } = rk;
                    try {
                        decryptedKeyStr = await this.crypto.decryptData<Uint8Array>(key, codeBytes, true);
                        if (decryptedKeyStr) {
                            break;
                        }
                    } catch (error) {
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