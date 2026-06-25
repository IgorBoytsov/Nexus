import { Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { StepLoginApi } from "./find-login.api";
import { firstValueFrom } from "rxjs";
import { RecoveryStateService } from "../../services/recovery-password-keys-state.service";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
    selector: 'app-find-login',
    templateUrl: './find-login.component.html',
    styleUrls: ['./find-login.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule]
})
export class StepFindLoginComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private http = inject(StepLoginApi);
    private state = inject(RecoveryStateService)

    stepLoginForm: FormGroup;
    isLoading = signal(false);
    errorMessage = signal<string | null>(null);

    constructor(){
        this.stepLoginForm = this.fb.group({
            login: ['', [Validators.required]]
        })
    }

    async onSubmit(): Promise<void> {
        if (this.stepLoginForm.valid){
            try {
                this.isLoading.set(true);
                this.errorMessage.set(null);
                
                const { login } = this.stepLoginForm.value;
                
                const result = await firstValueFrom(this.http.existLogin(login));

                if (result.isFailure){
                    this.errorMessage.set(result.stringMessage);
                    return;
                }

                this.state.setLogin(login);
                this.router.navigate(['code'], { relativeTo: this.route });
            } catch (error) {
                console.error('Ошибка: ', error);
            } finally{
                this.isLoading.set(false);
            }
        }
    }
}