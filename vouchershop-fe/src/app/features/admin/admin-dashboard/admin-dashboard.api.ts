import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AdminUpdateVoucherRequest {
  newAmount: number;
  newExpiresAtUtc: string; // ISO
}

export interface VoucherAuditDto {
  action: string;
  changesJson: string;
  performedByUserId: string;
  performedAt: string;
}

export interface AdminUserDto {
  id: string;
  email: string;
  userName?: string | null;
}

// ✅ Register tenant-aware (Admin)
export interface RegisterRequest {
  email: string;
  password: string;
}

export interface RegisterResponse {
  userId: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class AdminDashboardApi {
  private readonly baseUrl = 'https://localhost:7034/api';

  constructor(private http: HttpClient) {}

  updateVoucher(userId: string, req: AdminUpdateVoucherRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/admin/vouchers/${userId}`, req);
  }

  history(userId: string): Observable<VoucherAuditDto[]> {
    return this.http.get<VoucherAuditDto[]>(`${this.baseUrl}/admin/vouchers/${userId}/history`);
  }

  users(): Observable<AdminUserDto[]> {
    return this.http.get<AdminUserDto[]>(`${this.baseUrl}/admin/users`);
  }

  // ✅ la tua rotta BE: POST /api/admin/users/register
  register(req: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.baseUrl}/auth/register`, req);
  }
}
