import { Routes } from '@angular/router';
import { ShellComponent } from './layout/shell/shell';
import { LoginComponent } from './features/auth/login/login';
import { MyVoucherComponent } from './features/me/my-voucher/my-voucher';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard';

export const routes: Routes = [
  // login fuori dal layout
  {
  path: 'login',
  loadComponent: () =>
    import('./features/auth/login/login')
      .then(m => m.LoginComponent)
  },

  // layout principale
  {
    path: '',
    component: ShellComponent,
    children: [
      { path: 'me/voucher', component: MyVoucherComponent },
      { path: 'admin', component: AdminDashboardComponent },

      // redirect base
      { path: '', pathMatch: 'full', redirectTo: 'me/voucher' },
    ],
  },

  // fallback
  { path: '**', redirectTo: 'me/voucher' },
];
