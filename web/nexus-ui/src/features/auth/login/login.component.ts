import { Component, DestroyRef, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SrpClientService } from '@crossdyne/security'
import { AuthApi } from './auth.api';
import { firstValueFrom, Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Result } from '@crossdyne/toolkit';
import { CryptoConfigurationService } from '../../../core/services/crypto-configuration.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
})

export class LoginComponent {
  private readonly fb = inject(FormBuilder); 
  private readonly router = inject(Router); 
   private readonly route = inject(ActivatedRoute);
  private readonly authApi = inject(AuthApi);
  private readonly destroyRef = inject(DestroyRef);
  private cryptoConfig = inject(CryptoConfigurationService);

  private readonly srpService = new SrpClientService();

  loginForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  returnUtl: string = '/';

  readonly minLoginLength = 3;

  constructor() {
    this.loginForm = this.fb.group({
      login: ['', [Validators.required, Validators.minLength(this.minLoginLength)]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });

    this.returnUtl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid)
      return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const { srpContext } = await this.cryptoConfig.getSrpContext(); // Rfc5054_3072

    try {
      const { login: login, password } = this.loginForm.value;

      const challengeResult = await this.executeSafe(this.authApi.getCrpChallenge({ login: login }));

      if (challengeResult.isFailure){
        this.handleError(challengeResult)
        return;
      }

      const { salt, b } = challengeResult.value; 
      const {A, M1, S} = await this.srpService.generateSrpProof(login, password, salt, b, srpContext);

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

      const isServerValid = await this.srpService.verifyServerM2(A, M1, S, m2, srpContext);

      if (!isServerValid) {
        this.errorMessage.set("Ошибка аутентификации: Подлинность сервера не подтверждена!");
        return;
      }

      this.redirect();

    } catch (error) {
      console.error('Неизвестная ошибка:', error);
      this.errorMessage.set('Произошла непредвиденная ошибка.'); 
    } finally {
      this.isLoading.set(false);
    }
  }

  private redirect(): void {
    if (this.isValidReturnUrl(this.returnUtl)) {
      window.location.href = this.returnUtl;
    } else {
      this.router.navigate(['/user/profile']);
    }
  }

  private isValidReturnUrl(url: string): boolean {
    try {
      const parsedUrl = new URL(url);

      if (environment.production) {
        return parsedUrl.hostname.endsWith('.crossdyne.com')|| parsedUrl.hostname === 'crossdyne.com';
      }

      if (parsedUrl.hostname === 'localhost' || parsedUrl.hostname === '127.0.0.1') {
        return true;
      }

      return parsedUrl.hostname.endsWith('.crossdyne.com') || parsedUrl.hostname === 'crossdyne.com';
    } catch {
      return false;
    }
  }

  private executeSafe<T>(observable$: Observable<Result<T>>): Promise<Result<T>>{
    return firstValueFrom(observable$.pipe(takeUntilDestroyed(this.destroyRef)));
  }

  private handleError(result: Result<any>): void{
    this.errorMessage.set(result.errors.map(e => e.message).join('\n'));
  }
}