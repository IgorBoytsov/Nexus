import { Routes } from '@angular/router';
import { LoginComponent } from '../features/auth/login/login.component';
import { RegisterComponent } from '../features/auth/register/register.component';
import { MainLayoutComponent } from '../core/layout/main/main-layout.component';
import { ProfilePage } from '../features/profile/page/profile.page';
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
import { guestGuard } from '../core/guards/guest.guard';
import { ChangePasswordComponent } from '../features/profile/components/change-password/change-password.component';
import { ChangeEmailComponent } from '../features/profile/components/change-email/change-email.component';
import { changeEmailStepGuard } from '../features/profile/components/change-email/guards/change-email-step.guard';
import { ChangeEmailInitComponent } from '../features/profile/components/change-email/components/init/change-email-init.component';
import { ChangeEmailConfirmComponent } from '../features/profile/components/change-email/components/change/change-email-confirm.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: '',
    loadComponent: () => AuthLayoutComponent,
    children:[
      { path: 'login', loadComponent: () => LoginComponent, canActivate: [guestGuard] },
      { path: 'register', loadComponent: () => RegisterComponent, canActivate:[guestGuard]},
      {
        path: 'recovery/keys',
        component: RecoveryPasswordKeysComponent,
        canActivate: [guestGuard],
        children: [
          { path: '', component: StepFindLoginComponent, canActivate: [recoveryKeysStepGuard] },
          { path: 'code', component: StepEnterCodeComponent, canActivate: [recoveryKeysStepGuard] },
          { path: 'set', component: StepSetPasswordComponent, canActivate: [recoveryKeysStepGuard] },
          { path: '**', redirectTo: '' }
        ]
      },
      {
        path: 'change/email',
        component: ChangeEmailComponent,
        canActivate: [],
        children: [
          { path: '', component: ChangeEmailInitComponent, canActivate: [changeEmailStepGuard] },
          { path: 'confirm', component: ChangeEmailConfirmComponent, canActivate: [changeEmailStepGuard] },
          { path: '**', redirectTo: '' }
        ]
      },
      { 
        path: 'reset', 
        loadComponent: () => ResetComponent, 
        canActivate: [guestGuard],
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