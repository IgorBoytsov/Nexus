import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { StepCodeApi } from "./confirm-code.api";
import { ActivatedRoute, Router } from "@angular/router";
import { RecoveryStateService } from "../../services/reset-password-state.service";
import { firstValueFrom } from "rxjs";

@Component({
    selector: 'app-confirm-code',
    templateUrl: './confirm-code.component.html',
    styleUrls: ['./confirm-code.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule]
})
export class StepCodeComponent{
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private state = inject(RecoveryStateService);
    private api = inject(StepCodeApi);

    stepCodeConfirmForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    constructor() {
        this.stepCodeConfirmForm = this.fb.group({
            code: ['', [Validators.required]]
        })
    }

    async onSubmit() {
        if (this.stepCodeConfirmForm.valid) {
            try {
                this.isLoading.set(true);
                this.errorMessage.set(null);

                const { code } = this.stepCodeConfirmForm.value;

                var confirmResult = await firstValueFrom(this.api.verifyConfirmCode(this.state.login!, code));

                if (confirmResult.isFailure){
                    this.errorMessage.set(confirmResult.stringMessage)
                    return;
                }

                this.state.verifyCode(code);
                this.router.navigate(['set'], { relativeTo: this.route.parent} );
                
            } catch (error) {
                 console.error('Ошибка: ', error);
            } finally {
                this.isLoading.set(false);
            }
        }
    }
}