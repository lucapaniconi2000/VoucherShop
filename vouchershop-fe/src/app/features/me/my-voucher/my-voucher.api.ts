import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MeVoucherDto {
  amount: number;
  currency: string;
  updateAt: string;
  expiresAt: string;
  status: number;
}

@Injectable({ providedIn: 'root' })
export class MyVoucherApi {
  private readonly baseUrl = 'https://localhost:7034/api';

  constructor(private http: HttpClient) {}

  getMyVoucher(): Observable<MeVoucherDto> {
    return this.http.get<MeVoucherDto>(`${this.baseUrl}/me/voucher`);
  }
}
