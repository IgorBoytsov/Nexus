import { Routes } from '@angular/router';
import { LoginComponent } from '../features/auth/login/login.component';
import { RegisterComponent } from '../features/auth/register/register.component';
import { MainLayoutComponent } from '../core/layout/main/main-layout.component';
import { ProfilePage } from '../pages/profile/profile.page';
import { AuthLayoutComponent } from '../core/layout/auth/auth-layout.component';
import { ResetComponent } from '../features/auth/reset-password/reset-password.component';
import { StepLoginComponent } from '../features/auth/reset-password/components/find-login/find-login.component';
import { recoveryStepGuard } from '../features/auth/reset-password/guards/reset-password-step.guard';
import { StepCodeComponent } from '../features/auth/reset-password/components/confirm-code/confirm-code.component';
import { StepResetComponent } from '../features/auth/reset-password/components/reset/reset.component';
import { RecoveryPasswordKeysComponent } from '../features/auth/recovery-password-keys/recovery-password-keys.component';
import { StepFindLoginComponent } from '../features/auth/recovery-password-keys/components/find-login/find-login.component';
import { StepEnterCodeComponent } from '../features/auth/recovery-password-keys/components/enter-code/enter-code.component';
import { StepSetPasswordComponent } from '../features/auth/recovery-password-keys/components/set-password/set-password.component';
import { recoveryStepGuard as recoveryKeysStepGuard } from '../features/auth/recovery-password-keys/guards/recovery-password-keys.guard';
import { ChangePasswordComponent } from '../features/profile/change-password/change-password.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: '',
    loadComponent: () => AuthLayoutComponent,
    children:[
      { path: 'login', loadComponent: () => LoginComponent },
      { path: 'register', loadComponent: () => RegisterComponent },
      {
        path: 'recovery/keys',
        component: RecoveryPasswordKeysComponent,
        children: [
          { path: '', component: StepFindLoginComponent, canActivate: [recoveryKeysStepGuard] },
          { path: 'code', component: StepEnterCodeComponent, canActivate: [recoveryKeysStepGuard] },
          { path: 'set', component: StepSetPasswordComponent, canActivate: [recoveryKeysStepGuard] },
          { path: '**', redirectTo: '' }
        ]
      },
      { 
        path: 'reset', 
        loadComponent: () => ResetComponent, 
        children: [
          { path: '', component: StepLoginComponent, canActivate: [recoveryStepGuard] },
          { path: 'code', component: StepCodeComponent, canActivate: [recoveryStepGuard] },
          { path: 'set', component: StepResetComponent, canActivate: [recoveryStepGuard] },
          { path: '**', redirectTo: '' }
        ]
      }
    ]
  },
  { 
    path: 'user',
    loadComponent: () => MainLayoutComponent,
    children: [
      { path: 'profile', loadComponent: () => ProfilePage },
      { path: 'change/password', loadComponent: () => ChangePasswordComponent},
      { path: '', redirectTo: 'profile', pathMatch: 'full' }
    ]
  },
];