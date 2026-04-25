import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SrpService } from '@quantropic/security'
import { SrpChallengeRequest } from '../../../contracts/requests/srp-challenge.request';
import { SrpVerifyRequest } from '../../../contracts/requests/srp-verify.request';
import { AuthApi } from './auth.api';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule],
})

export class LoginComponent {
  private fb = inject(FormBuilder); 
  private router = inject(Router); 
  private authApi = inject(AuthApi);

  loginForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;

  readonly minUsernameLength = 2;

  constructor() {
    this.loginForm = this.fb.group({
      login: ['', [Validators.required, Validators.minLength(this.minUsernameLength)]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  srpService = new SrpService();

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid)
      return;
    const { login: login, password } = this.loginForm.value;

    const srpChallengeRequest: SrpChallengeRequest = { login: login };
    const srpChallengeResponse = await firstValueFrom(this.authApi.getCrpChallenge(srpChallengeRequest));
    const { salt, b } = srpChallengeResponse; 
    const {A, M1, S} = await this.srpService.generateSrpProof(login, password, salt, b);

    const srpVerifyRequest: SrpVerifyRequest = { Login: login, A, M1};

    const srpVerifierResponse = await firstValueFrom(this.authApi.srpVerifyProof(srpVerifyRequest));
    const { m2 } = srpVerifierResponse;

    if (!m2){
      this.errorMessage = "Ошибка аутентификации: M2 отсутствует в ответе сервера.";
      return;
    }

    const isServerValid = await this.srpService.verifyServerM2(A, M1, S, m2);

    if (!isServerValid) {
      this.errorMessage = "Ошибка аутентификации: Подлинность сервера не подтверждена!";
      return;
    }

    console.log("Успешная аутентификация! Сервер подтвержден.");

    this.isLoading = true;
    this.errorMessage = null;

    this.router.navigate(['/user/profile'])
  }
}