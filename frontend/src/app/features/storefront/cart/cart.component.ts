import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartLine, CartStore } from '../data/cart-store.service';
import { EXPEDITION_OPTIONS } from '../data/expedition';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent {
  readonly cart = inject(CartStore);
  readonly expeditionOptions = EXPEDITION_OPTIONS;

  updateQty(line: CartLine, qty: number): void {
    this.cart.updateQty(line.product, line.variant, qty);
  }

  setExpedition(line: CartLine, courier: string): void {
    this.cart.setExpedition(line.product, line.variant, courier);
  }

  remove(line: CartLine): void {
    this.cart.remove(line.product, line.variant);
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', maximumFractionDigits: 0 }).format(value);
  }
}
