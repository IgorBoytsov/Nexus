import { Component, DestroyRef, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SrpService } from '@crossdyne/security'
import { AuthApi } from './auth.api';
import { firstValueFrom, Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Result } from '@crossdyne/toolkit';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule],
})

export class LoginComponent {
  private readonly fb = inject(FormBuilder); 
  private readonly router = inject(Router); 
  private readonly authApi = inject(AuthApi);
  private readonly destroyRef = inject(DestroyRef);
  private readonly srpService = new SrpService();

  loginForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  readonly minUsernameLength = 2;

  constructor() {
    this.loginForm = this.fb.group({
      login: ['', [Validators.required, Validators.minLength(this.minUsernameLength)]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid)
      return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      const { login: login, password } = this.loginForm.value;

      const challengeResult = await this.executeSafe(this.authApi.getCrpChallenge({ login: login }));

      if (challengeResult.isFailure){
        this.handleError(challengeResult)
        return;
      }

      const { salt, b } = challengeResult.value; 
      const {A, M1, S} = await this.srpService.generateSrpProof(login, password, salt, b);

      const verifierResult = await this.executeSafe(this.authApi.srpVerifyProof({ Login: login, A, M1}));
      
      if (verifierResult.isFailure){
        this.handleError(verifierResult);
        return;
      }

      const { m2 } = verifierResult.value;

      if (!m2){
        this.errorMessage.set("Ошибка аутентификации: M2 отсутствует в ответе сервера.");
        return;
      }

      const isServerValid = await this.srpService.verifyServerM2(A, M1, S, m2);

      if (!isServerValid) {
        this.errorMessage.set("Ошибка аутентификации: Подлинность сервера не подтверждена!");
        return;
      }

      console.log("Успешная аутентификация! Сервер подтвержден.");

      this.router.navigate(['/user/profile'])
    } catch (error) {
      console.error('Неизвестная ошибка:', error);
      this.errorMessage.set('Произошла непредвиденная ошибка.'); 
    } finally {
      this.isLoading.set(false);
    }
  }

  private executeSafe<T>(observable$: Observable<Result<T>>): Promise<Result<T>>{
    return firstValueFrom(observable$.pipe(takeUntilDestroyed(this.destroyRef)));
  }

  private handleError(result: Result<any>): void{
    this.errorMessage.set(result.errors.map(e => e.message).join('\n'));
  }
}