import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CartStore } from '../data/cart-store.service';
import { OrderService } from '../data/order.service';
import { WalletService } from '../data/wallet.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent {
  readonly cart = inject(CartStore);
  private readonly orderService = inject(OrderService);
  private readonly walletService = inject(WalletService);
  private readonly router = inject(Router);

  readonly walletBalance = signal<number | null>(null);
  readonly placing = signal(false);
  readonly topUpAmount = signal(100_000);
  readonly error = signal<string | null>(null);
  readonly insufficientBalance = signal(false);

  constructor() {
    this.refreshBalance();
  }

  refreshBalance(): void {
    this.walletService.getBalance().subscribe((balance) => this.walletBalance.set(balance));
  }

  setTopUpAmount(value: string): void {
    const n = Number(value);
    this.topUpAmount.set(Number.isFinite(n) && n > 0 ? n : 0);
  }

  topUp(): void {
    if (this.topUpAmount() <= 0) return;
    this.walletService.topUp(this.topUpAmount()).subscribe((balance) => {
      this.walletBalance.set(balance);
      this.insufficientBalance.set(false);
      this.error.set(null);
    });
  }

  placeOrder(): void {
    if (this.cart.lines().length === 0) return;
    this.placing.set(true);
    this.error.set(null);
    this.insufficientBalance.set(false);

    const items = this.cart.lines().map((l) => ({
      productId: l.product.id,
      quantity: l.qty,
      expeditionCourier: l.expeditionCourier,
    }));

    this.orderService.checkout(items).subscribe({
      next: () => {
        this.cart.clear();
        this.placing.set(false);
        this.router.navigateByUrl('/storefront/orders');
      },
      error: (err) => {
        this.placing.set(false);
        const message: string = err.error?.error ?? 'Checkout gagal. Coba lagi.';
        this.error.set(message);
        this.refreshBalance();

        if (message.includes('Saldo wallet tidak cukup')) {
          this.insufficientBalance.set(true);
          const shortfall = this.cart.subtotal() - (this.walletBalance() ?? 0);
          this.topUpAmount.set(Math.max(10_000, Math.ceil(shortfall / 1000) * 1000));
        }
      },
    });
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', maximumFractionDigits: 0 }).format(value);
  }
}
