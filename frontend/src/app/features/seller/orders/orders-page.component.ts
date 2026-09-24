import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Order } from '../../storefront/data/order.model';
import { OrderService } from '../../storefront/data/order.service';

@Component({
  selector: 'app-seller-orders-page',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './orders-page.component.html',
  styleUrl: './orders-page.component.scss',
})
export class SellerOrdersPageComponent {
  private readonly orderService = inject(OrderService);

  readonly orders = signal<Order[]>([]);
  readonly busyOrderId = signal<string | null>(null);

  constructor() {
    this.refresh();
  }

  refresh(): void {
    this.orderService.listSellerInbox().subscribe((orders) => this.orders.set(orders));
  }

  ship(order: Order): void {
    this.busyOrderId.set(order.id);
    this.orderService.markShipped(order.id).subscribe({
      next: () => {
        this.busyOrderId.set(null);
        this.refresh();
      },
      error: () => this.busyOrderId.set(null),
    });
  }

  deliver(order: Order): void {
    this.busyOrderId.set(order.id);
    this.orderService.markDelivered(order.id).subscribe({
      next: () => {
        this.busyOrderId.set(null);
        this.refresh();
      },
      error: () => this.busyOrderId.set(null),
    });
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', maximumFractionDigits: 0 }).format(value);
  }
}
