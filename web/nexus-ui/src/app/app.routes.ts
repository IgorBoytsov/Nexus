import { Routes } from '@angular/router';
import { MainLayoutComponent } from '../core/layout/main/main-layout.component';
import { ProfilePage } from '../features/profile/page/profile.page';
import { ChangePasswordComponent } from '../features/profile/components/change-password/change-password.component';
import { ChangeEmailComponent } from '../features/profile/components/change-email/change-email.component';
import { ChangeEmailInitComponent } from '../features/profile/components/change-email/components/init/change-email-init.component';
import { changeEmailStepGuard } from '../features/profile/components/change-email/guards/change-email-step.guard';
import { ChangeEmailConfirmComponent } from '../features/profile/components/change-email/components/change/change-email-confirm.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: '',
    loadChildren: () => import('../features/authentication/authentication.routes').then(r => r.AUTHENTICATION_ROUTES)
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
];