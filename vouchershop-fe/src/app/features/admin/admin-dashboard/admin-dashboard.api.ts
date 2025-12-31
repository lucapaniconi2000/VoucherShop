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
export interface CreateUserRequest {
  email: string;
  password: string;
}

export interface CreateUserResponse {
  userId: string;
  email: string;
}

export interface AdminVoucherDto {
  userId: string;
  amount: number;
  currency: string;
  updatedAt: string;
  expiresAt: string;
  isExpired: boolean;
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

  createUser(req: CreateUserRequest) : Observable<CreateUserResponse> {
    return this.http.post<CreateUserResponse>(`${this.baseUrl}/admin/users`, req);
  }

  voucher(userId: string) : Observable<AdminVoucherDto> {
    return this.http.get<AdminVoucherDto>(`${this.baseUrl}/admin/vouchers/${userId}`);
  }
}
