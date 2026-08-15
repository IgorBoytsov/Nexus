import { Component, DestroyRef, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CryptoVersion, SecurityUtils, SrpClientService, SrpGroup, SrpKeyDerivationService } from '@crossdyne/security'
import { AuthApi } from './auth.api';
import { firstValueFrom, Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Result } from '@crossdyne/toolkit';
import { environment } from '../../../environments/environment';
import { CryptoConstants } from '../../../core/constants/security.constants';
import { SrpChallengeResponse } from './models/srp-challenge.response';

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

  private readonly srpKeyDerivationService = new SrpKeyDerivationService();
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

    this.route.queryParams.subscribe(params => {
      this.returnUtl = params['returnUrl'] || '/';
    });
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid)
      return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      const { password } = this.loginForm.value;
      const rawLogin = this.loginForm.value.login as string;
      const normalizedLogin = rawLogin.trim().toLowerCase();

      const challengeResult: Result<SrpChallengeResponse> = await this.executeSafe(this.authApi.getCrpChallenge({ login: normalizedLogin }));

      if (challengeResult.isFailure){
        this.handleError(challengeResult)
        return;
      }

      const { salt, b, srpVersion, srpCryptoVersion } = challengeResult.value; 
      const saltBytes = SecurityUtils.fromBase64(salt);
      const authHashBytes = await this.srpKeyDerivationService.deriveAuthHashForSrp(normalizedLogin, password, saltBytes, srpVersion as SrpGroup, srpCryptoVersion as CryptoVersion);
      const {A, M1, SessionKeyK} = await this.srpService.generateSrpProof(normalizedLogin, authHashBytes, salt, b, srpVersion as SrpGroup);

      const verifierResult = await this.executeSafe(this.authApi.srpVerifyProof({ Login: normalizedLogin, A, M1}));
      
      if (verifierResult.isFailure){
        this.handleError(verifierResult);
        return;
      }

      const { m2, tempAuthToken } = verifierResult.value;

      if (!m2){
        this.errorMessage.set("Ошибка аутентификации: M2 отсутствует в ответе сервера.");
        return;
      }

      const isServerValid = await this.srpService.verifyServerM2(A, M1, SessionKeyK, m2, srpVersion as SrpGroup);

      if (!isServerValid) {
        this.errorMessage.set("Ошибка аутентификации: Подлинность сервера не подтверждена!");
        return;
      }

      const complete = await this.executeSafe(this.authApi.srpComplete({ tempAuthToken }));

      if (complete.isFailure){
        this.handleError(complete);
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
    if (!this.returnUtl || this.returnUtl === '/') {
      this.router.navigate(['/user/profile']);
      return;
    }

    if (this.isValidReturnUrl(this.returnUtl)) {
      if (this.returnUtl.startsWith('http://') || this.returnUtl.startsWith('https://')) {
        window.location.href = this.returnUtl;
      } else {
        this.router.navigateByUrl(this.returnUtl);
      }
    } else {
      this.router.navigate(['/user/profile']);
    }
  }

  private isValidReturnUrl(url: string): boolean {
    if (!url) return false;
    
    if (url.startsWith('/')) {
      return true;
    }
    
    try {
      const parsedUrl = new URL(url);
      if (environment.production) {
        return parsedUrl.hostname.endsWith('.crossdyne.com') || parsedUrl.hostname === 'crossdyne.com';
      }
      return parsedUrl.hostname === 'localhost' || parsedUrl.hostname === '127.0.0.1' ||
            parsedUrl.hostname.endsWith('.crossdyne.com') || parsedUrl.hostname === 'crossdyne.com';
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