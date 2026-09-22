import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { AuthStore } from '../../../../core/auth/auth-store.service';

interface IncomingOrder {
  id: string;
  buyer: string;
  total: number;
  status: 'Pending' | 'Paid' | 'Shipped' | 'Completed';
}

@Component({
  selector: 'app-my-store-page',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './my-store-page.component.html',
  styleUrl: './my-store-page.component.scss',
})
export class MyStorePageComponent {
  private readonly auth = inject(AuthStore);

  readonly storeName = () => this.auth.user()?.storeName ?? 'My Store';

  // Placeholder incoming orders — Ordering module isn't wired up server-side yet.
  readonly incomingOrders = signal<IncomingOrder[]>([
    { id: 'ORD-2091', buyer: 'buyer1@example.com', total: 185000, status: 'Pending' },
    { id: 'ORD-2077', buyer: 'buyer2@example.com', total: 130000, status: 'Paid' },
  ]);

  readonly showProductForm = signal(false);

  toggleProductForm(): void {
    this.showProductForm.update((v) => !v);
  }
}
