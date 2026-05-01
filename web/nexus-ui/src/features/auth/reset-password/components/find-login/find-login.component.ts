import { Component, inject } from "@angular/core";
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
    isLoading = false;
    errorMessage: string | null = null;

    constructor(){
        this.stepLoginForm = this.fb.group({
            login: ['', [Validators.required]]
        });
    }

    async onSubmit(): Promise<void> {
        if (this.stepLoginForm.valid){
            try{
                const { login } = this.stepLoginForm.value;

                await firstValueFrom(this.api.generateCode(login));

                this.state.setLogin(login);
                this.router.navigate(['code'], {relativeTo: this.route} );

            } catch (error) {
                console.error('Ошибка: ', error);
            }
        }
    }
}