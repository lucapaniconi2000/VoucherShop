import { CanMatchFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const authGuard: CanMatchFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.token ? true : router.parseUrl('/login');
};

export function roleGuard(required: 'Admin' | 'User'): CanMatchFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!auth.token) return router.parseUrl('/login');
    if (auth.roles.includes(required)) return true;

    // loggato ma ruolo sbagliato → mandalo alla home corretta
    return router.parseUrl(auth.defaultRoute());
  };
}

// utile: se uno è già loggato e va su /login → rimandalo alla sua home
export const redirectIfAuthenticatedGuard: CanMatchFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.token ? router.parseUrl(auth.defaultRoute()) : true;
};