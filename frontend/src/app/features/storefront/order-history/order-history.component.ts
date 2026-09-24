import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { OrderService } from '../data/order.service';
import { OrderStatus } from '../data/order.model';

const STAGES: OrderStatus[] = ['Pending', 'Paid', 'Shipped', 'Completed'];

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './order-history.component.html',
  styleUrl: './order-history.component.scss',
})
export class OrderHistoryComponent {
  private readonly orderService = inject(OrderService);
  readonly orders = toSignal(this.orderService.list(), { initialValue: [] });
  readonly stages = STAGES;

  stageIndex(status: OrderStatus): number {
    return STAGES.indexOf(status);
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', maximumFractionDigits: 0 }).format(value);
  }
}
