import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { RecoveryStateService } from "../../services/reset-password-state.service";
import { StepLoginApi } from "./find-login.api";
import { firstValueFrom } from "rxjs";

@Component({
    selector: 'app-find-login',
    templateUrl: './find-login.component.html',
    styleUrls: ['./find-login.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule]
})
export class StepLoginComponent{
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private state = inject(RecoveryStateService);
    private api = inject(StepLoginApi);

    stepLoginForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    constructor(){
        this.stepLoginForm = this.fb.group({
            login: ['', [Validators.required]]
        });
    }

    async onSubmit(): Promise<void> {
        if (this.stepLoginForm.valid){
            try{
                this.isLoading.set(true);
                this.errorMessage.set(null);

                const login = this.stepLoginForm.value.login as string;
                const normalizeLogin = login.trim().toLowerCase();

                const codeResult = await firstValueFrom(this.api.generateCode(normalizeLogin));

                if (codeResult.isFailure){
                    this.errorMessage.set(codeResult.stringMessage)
                    return;
                }

                this.state.setLogin(normalizeLogin);
                this.router.navigate(['code'], {relativeTo: this.route} );

            } catch (error) {
                console.error('Ошибка: ', error);
            }
            finally{
                this.isLoading.set(false);
            }
        }
    }
}