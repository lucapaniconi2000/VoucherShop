import { Routes } from '@angular/router';
import { ShellComponent } from './layout/shell/shell';
import { authGuard, roleGuard } from './core/auth/auth.guards';

export const routes: Routes = [
  {
    path: '',
    component: ShellComponent,
    children: [
      // PUBLIC
      {
        path: 'store/:shopId',
        loadComponent: () =>
          import('./features/store/store-landing/store-landing')
            .then(m => m.StoreLandingComponent)
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login')
            .then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/register/register')
            .then(m => m.RegisterComponent)
      },

      // PROTECTED
      {
        path: 'me/voucher',
        canMatch: [authGuard, roleGuard('User')],
        loadComponent: () =>
          import('./features/me/my-voucher/my-voucher')
            .then(m => m.MyVoucherComponent)
      },
      {
        path: 'admin',
        canMatch: [authGuard, roleGuard('Admin')],
        loadComponent: () =>
          import('./features/admin/admin-dashboard/admin-dashboard')
            .then(m => m.AdminDashboardComponent)
      },

      // HOME
      { path: '', pathMatch: 'full', redirectTo: 'login' },

      // FALLBACK
      { path: '**', redirectTo: 'login' },
    ],
  },
];
