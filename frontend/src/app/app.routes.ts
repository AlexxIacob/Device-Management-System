import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'welcome', loadComponent: () => import('./pages/welcome/welcome').then(m => m.WelcomeComponent) },
  { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./pages/register/register').then(m => m.RegisterComponent) },
  { path: 'profile', loadComponent: () => import('./pages/profile/profile').then(m => m.ProfileComponent), canActivate: [authGuard] },
  { path: 'home', loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent), canActivate: [authGuard] },
  { path: 'devices', loadComponent: () => import('./pages/my-devices/my-devices').then(m => m.MyDevicesComponent), canActivate: [authGuard] },
  { path: 'my-profile', loadComponent: () => import('./pages/my-profile/my-profile').then(m => m.MyProfileComponent), canActivate: [authGuard] },
  { path: 'devices/new', loadComponent: () => import('./forms/device.form/device.form').then(m => m.DeviceFormComponent), canActivate: [authGuard] },
  { path: 'devices/:id/edit', loadComponent: () => import('./forms/device.form/device.form').then(m => m.DeviceFormComponent), canActivate: [authGuard] },
];