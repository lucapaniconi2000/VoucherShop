import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, shareReplay, switchMap, throwError, Observable } from 'rxjs';
import { AuthService } from './auth.service';

let refreshInFlight$: Observable<{ accessToken: string }> | null = null;

function isAuthEndpoint(url: string): boolean {
  return url.includes('/api/auth/');
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const token = auth.token;
  const authCall = isAuthEndpoint(req.url);

  // evita di attaccare token alle chiamate /auth/*
  const requestToSend =
    token && !authCall
      ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : req;

  return next(requestToSend).pipe(
    catchError((err) => {
      // refresh solo su 401
      if (!(err instanceof HttpErrorResponse) || err.status !== 401 || authCall) {
        return throwError(() => err);
      }

      // se già ritentata una volta, non loopare
      const alreadyRetried = req.headers.has('x-refresh-retried');
      if (alreadyRetried) {
        auth.logout();
        router.navigateByUrl('/login');
        return throwError(() => err);
      }

      // una sola refresh in parallelo
      if (!refreshInFlight$) {
        refreshInFlight$ = auth.refresh().pipe(
          shareReplay(1),
          finalize(() => (refreshInFlight$ = null))
        );
      }

      return refreshInFlight$.pipe(
        switchMap(() => {
          const newToken = auth.token;
          if (!newToken) {
            auth.logout();
            router.navigateByUrl('/login');
            return throwError(() => err);
          }

          // retry della request originale con header di guardia anti-loop
          const retryReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${newToken}`,
              'x-refresh-retried': '1',
            },
          });

          return next(retryReq);
        }),
        catchError((refreshErr) => {
          auth.logout();
          router.navigateByUrl('/login');
          return throwError(() => refreshErr);
        })
      );
    })
  );
};
