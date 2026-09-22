import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CartStore } from '../data/cart-store.service';
import { OrderService } from '../data/order.service';
import { PaymentMethod } from '../data/order.model';

@Component({
  selector: 'app-checkout',
  standalone: true,
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent {
  readonly cart = inject(CartStore);
  private readonly orderService = inject(OrderService);
  private readonly router = inject(Router);

  readonly paymentMethod = signal<PaymentMethod>('transfer');
  readonly placing = signal(false);

  selectPayment(method: PaymentMethod): void {
    this.paymentMethod.set(method);
  }

  placeOrder(): void {
    if (this.cart.lines().length === 0) return;
    this.placing.set(true);
    this.orderService.placeOrder(this.cart.lines(), this.paymentMethod()).subscribe((order) => {
      this.cart.clear();
      this.placing.set(false);
      this.router.navigateByUrl('/storefront/orders');
    });
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', maximumFractionDigits: 0 }).format(value);
  }
}
