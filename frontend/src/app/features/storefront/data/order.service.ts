import { Injectable, signal } from '@angular/core';
import { Observable, delay, of } from 'rxjs';
import { CartLine } from './cart-store.service';
import { Order, PaymentMethod } from './order.model';

// Placeholder: backend Ordering module isn't wired up yet, so orders live in an in-memory signal; swap by replacing placeOrder/list bodies with HttpClient calls to ordering/orders.
@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly _orders = signal<Order[]>([
    {
      id: 'ORD-1042',
      placedAt: '2026-09-10T09:12:00Z',
      status: 'Completed',
      paymentMethod: 'transfer',
      items: [{ productName: 'Gentong Tanah Liat Klasik', variantLabel: '10 Liter', qty: 1, price: 185000 }],
      total: 185000,
    },
    {
      id: 'ORD-1051',
      placedAt: '2026-09-18T14:30:00Z',
      status: 'Shipped',
      paymentMethod: 'qris',
      items: [{ productName: 'Kendi Air Minum', variantLabel: 'Standar', qty: 2, price: 65000 }],
      total: 130000,
    },
  ]);

  list(): Observable<Order[]> {
    return of(this._orders()).pipe(delay(150));
  }

  placeOrder(lines: CartLine[], paymentMethod: PaymentMethod): Observable<Order> {
    const order: Order = {
      id: `ORD-${Math.floor(1000 + Math.random() * 9000)}`,
      placedAt: new Date().toISOString(),
      status: 'Pending',
      paymentMethod,
      items: lines.map((l) => ({
        productName: l.product.name,
        variantLabel: l.variant.label,
        qty: l.qty,
        price: l.product.price + l.variant.priceDelta,
      })),
      total: lines.reduce((sum, l) => sum + (l.product.price + l.variant.priceDelta) * l.qty, 0),
    };
    this._orders.update((orders) => [order, ...orders]);
    return of(order).pipe(delay(200));
  }
}
