import { Routes } from '@angular/router';
import { LoginComponent } from '../features/auth/login/login.component';
import { RegisterComponent } from '../features/registration/register/register.component';
import { MainLayoutComponent } from '../core/layout/main/main-layout.component';
import { ProfilePage } from '../pages/profile/profile.page';
import { AuthLayoutComponent } from '../core/layout/auth/auth-layout.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: '',
    loadComponent: () => AuthLayoutComponent,
    children:[
      { path: 'login', loadComponent: () => LoginComponent },
      { path: 'register', loadComponent: () => RegisterComponent },
    ]
  },
  { 
    path: 'user',
    loadComponent: () => MainLayoutComponent,
    children: [
      { path: 'profile', loadComponent: () => ProfilePage },
      { path: '', redirectTo: 'profile', pathMatch: 'full' }
    ]
  },
];