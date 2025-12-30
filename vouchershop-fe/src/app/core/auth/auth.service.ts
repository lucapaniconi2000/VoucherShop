import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginRequest, LoginResponse } from './auth.models';
import { Observable, tap } from 'rxjs';
import { parseJwt, getJwtRoles } from './jwt.utils';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:7034/api/auth';

  // ✅ cache in memoria (evita di ricalcolare ogni volta)
  private _roles: string[] = [];

  constructor(private http: HttpClient) {
    // ✅ se c'è già un token salvato, inizializza i ruoli all'avvio
    const token = this.token;
    if (token) this.setToken(token);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, request, {
        withCredentials: true // 🔑 refresh token cookie
      })
      .pipe(
        tap(res => {
          this.setToken(res.accessToken);
        })
      );
  }

  logout() {
    localStorage.removeItem('accessToken');
    this._roles = [];
  }

  get token(): string | null {
    return localStorage.getItem('accessToken');
  }

  // ✅ ruoli letti dal JWT
  get roles(): string[] {
    return this._roles;
  }

  hasRole(role: 'Admin' | 'User'): boolean {
    return this._roles.includes(role);
  }

  // ✅ route “giusta” post-login
  defaultRoute(): string {
    return this.hasRole('Admin') ? '/admin' : '/me/voucher';
  }

  // --- helper privato ---
  private setToken(token: string) {
    localStorage.setItem('accessToken', token);

    const payload = parseJwt(token);
    this._roles = getJwtRoles(payload);

    // (debug utile una volta sola)
    // console.log('JWT roles:', this._roles, 'payload:', payload);
  }

  refresh(): Observable<{ accessToken: string }> {
    return this.http.post<{ accessToken: string }>(
      `${this.apiUrl}/refresh`,
      {},
      { withCredentials: true } // ✅ serve per inviare il cookie refreshToken
    ).pipe(
      tap(res => this.setToken(res.accessToken)) // riusa la tua setToken()
    );
  }

  selfRegister(req: { shopId: string; email: string; password: string }) {
    return this.http.post<{ userId: string; email: string; shopId: string }>(
      `${this.apiUrl}/self-register`,
      req
    );
  }
}
