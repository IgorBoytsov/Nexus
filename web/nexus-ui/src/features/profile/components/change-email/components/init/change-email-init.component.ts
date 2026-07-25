import { Component, inject, signal } from "@angular/core";
import { ChangeEmailStateService } from "../../services/change-email-state.service";
import { ChangeEmailService } from "../../services/change-email.service";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Result } from "@crossdyne/toolkit";
import { ChangeEmailInitRequest } from "../../models/change-email-init.request";
import { MapErrorsHelper } from "../../../../../../core/helpers/map-errors.helper";

@Component({
    selector: 'change-email-init',
    templateUrl: './change-email-init.component.html',
    styleUrls: ['./change-email-init.component.scss'],
    standalone: true,
    imports: [
        ReactiveFormsModule
    ]
})
export class ChangeEmailInitComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private changeEmailService = inject(ChangeEmailService);
    private state = inject(ChangeEmailStateService);

    stepInitForm: FormGroup = this.fb.group({
        email: ['', [Validators.required]]
    });
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    async onSubmit(): Promise<void> {
        if (this.stepInitForm.valid){
            try {
                this.isLoading.set(true);
                this.errorMessage.set(null);

                const email = this.stepInitForm.value.email as string;
                
                this.state.setEmail(email);

                const request: ChangeEmailInitRequest = {
                    email: email
                }
                
                const codeResult: Result<string> = await this.changeEmailService.init(request);

                codeResult.switch(
                    () => this.router.navigate(['confirm'], { relativeTo: this.route }),
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