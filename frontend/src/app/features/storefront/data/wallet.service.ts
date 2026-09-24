import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class WalletService {
  private readonly http = inject(HttpClient);

  getBalance(): Observable<number> {
    return this.http
      .get<{ balance: number }>(`${environment.apiUrl}/wallet/balance`)
      .pipe(map((r) => r.balance));
  }

  topUp(amount: number): Observable<number> {
    return this.http
      .post<{ balance: number }>(`${environment.apiUrl}/wallet/topup`, { amount })
      .pipe(map((r) => r.balance));
  }
}
