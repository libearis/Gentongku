import { Component, inject, signal } from '@angular/core';
import { WalletService } from '../data/wallet.service';

@Component({
  selector: 'app-wallet',
  standalone: true,
  templateUrl: './wallet.component.html',
  styleUrl: './wallet.component.scss',
})
export class WalletComponent {
  private readonly walletService = inject(WalletService);

  readonly balance = signal<number | null>(null);
  readonly topUpAmount = signal(100_000);
  readonly toppingUp = signal(false);
  readonly justToppedUp = signal(false);

  constructor() {
    this.refresh();
  }

  refresh(): void {
    this.walletService.getBalance().subscribe((balance) => this.balance.set(balance));
  }

  setTopUpAmount(value: string): void {
    const n = Number(value);
    this.topUpAmount.set(Number.isFinite(n) && n > 0 ? n : 0);
  }

  topUp(): void {
    if (this.topUpAmount() <= 0) return;
    this.toppingUp.set(true);
    this.walletService.topUp(this.topUpAmount()).subscribe((balance) => {
      this.balance.set(balance);
      this.toppingUp.set(false);
      this.justToppedUp.set(true);
      setTimeout(() => this.justToppedUp.set(false), 2000);
    });
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', maximumFractionDigits: 0 }).format(value);
  }
}
