import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginRequest, LoginResponse } from './auth.models';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:7034/api/auth';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, request, {
        withCredentials: true // 🔑 refresh token cookie
      })
      .pipe(
        tap(res => {
          localStorage.setItem('accessToken', res.accessToken);
        })
      );
  }

  logout() {
    localStorage.removeItem('accessToken');
  }

  get token(): string | null {
    return localStorage.getItem('accessToken');
  }
}
