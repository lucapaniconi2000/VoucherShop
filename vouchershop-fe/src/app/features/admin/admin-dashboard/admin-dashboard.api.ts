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
}
