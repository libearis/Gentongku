import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CheckoutItem, Order } from './order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);

  list(): Observable<Order[]> {
    return this.http.get<Order[]>(`${environment.apiUrl}/orders/buyer/me`);
  }

  checkout(items: CheckoutItem[]): Observable<Order[]> {
    return this.http.post<Order[]>(`${environment.apiUrl}/orders/checkout`, { items });
  }

  listSellerInbox(): Observable<Order[]> {
    return this.http.get<Order[]>(`${environment.apiUrl}/orders/seller/inbox`);
  }

  markShipped(orderId: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/orders/${orderId}/ship`, {});
  }

  markDelivered(orderId: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/orders/${orderId}/deliver`, {});
  }
}
