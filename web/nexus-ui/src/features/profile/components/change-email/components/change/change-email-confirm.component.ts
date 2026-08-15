import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ChangeEmailService } from "../../services/change-email.service";
import { ChangeEmailStateService } from "../../services/change-email-state.service";
import { ChangeEmailRequest } from "../../models/change-email.request";
import { Result, Unit } from "@crossdyne/toolkit";
import { MapErrorsHelper } from "../../../../../../core/helpers/map-errors.helper";

@Component({
    selector: 'change-email-confirm',
    templateUrl: './change-email-confirm.component.html',
    styleUrls: ['./change-email-confirm.component.scss'],
    standalone: true,
    imports: [
        ReactiveFormsModule
    ]
})
export class ChangeEmailConfirmComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private changeEmailService = inject(ChangeEmailService);
    private state = inject(ChangeEmailStateService);

    stepConfirmForm: FormGroup = this.fb.group({
        code: ['', [Validators.required]]
    });
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    async onSubmit(): Promise<void> {
        if (this.stepConfirmForm.valid){
            try {
                this.isLoading.set(true);
                this.errorMessage.set(null);

                const code = this.stepConfirmForm.value.code as string;

                const request: ChangeEmailRequest = {
                    email: this.state.email!,
                    code: code,
                }

                const result: Result<Unit> = await this.changeEmailService.change(request);

                result.switch(
                    _ => this.router.navigate(['/user/profile']),
                    errors => console.error(MapErrorsHelper.mapErrors(errors))
                );
            } catch (error) {
                console.error('Ошибка: ', error);
            } finally {
                this.isLoading.set(false);
            }
        }
    }
}