import { Injectable } from '@angular/core';
import { Observable, shareReplay, finalize, map } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class RefreshTokenService {
  private inFlight$: Observable<string> | null = null;

  constructor(private auth: AuthService) {}

  refreshAccessToken(): Observable<string> {
    if (!this.inFlight$) {
      this.inFlight$ = this.auth.refresh().pipe(
        map(r => r.accessToken),
        shareReplay(1),
        finalize(() => (this.inFlight$ = null))
      );
    }
    return this.inFlight$;
  }
}
