import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartLine, CartStore } from '../data/cart-store.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent {
  readonly cart = inject(CartStore);

  updateQty(line: CartLine, qty: number): void {
    this.cart.updateQty(line.product, line.variant, qty);
  }

  remove(line: CartLine): void {
    this.cart.remove(line.product, line.variant);
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', maximumFractionDigits: 0 }).format(value);
  }
}
