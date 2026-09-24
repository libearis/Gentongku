import { Injectable, computed, signal } from '@angular/core';
import { EXPEDITION_OPTIONS } from './expedition';
import { Product, ProductVariant } from './product.model';

export interface CartLine {
  product: Product;
  variant: ProductVariant;
  qty: number;
  expeditionCourier: string;
}

function expeditionCost(courier: string): number {
  return EXPEDITION_OPTIONS.find((o) => o.code === courier)?.cost ?? 0;
}

@Injectable({ providedIn: 'root' })
export class CartStore {
  private readonly _lines = signal<CartLine[]>([]);

  readonly lines = this._lines.asReadonly();
  readonly itemCount = computed(() => this._lines().reduce((sum, l) => sum + l.qty, 0));
  readonly subtotal = computed(() =>
    this._lines().reduce((sum, l) => sum + (l.product.price + l.variant.priceDelta) * l.qty + expeditionCost(l.expeditionCourier), 0),
  );

  add(product: Product, variant: ProductVariant, qty: number): void {
    const existing = this._lines().find((l) => l.product.id === product.id && l.variant.id === variant.id);
    if (existing) {
      this._lines.update((lines) =>
        lines.map((l) => (l === existing ? { ...l, qty: l.qty + qty } : l)),
      );
      return;
    }
    this._lines.update((lines) => [...lines, { product, variant, qty, expeditionCourier: EXPEDITION_OPTIONS[0].code }]);
  }

  updateQty(product: Product, variant: ProductVariant, qty: number): void {
    if (qty <= 0) {
      this.remove(product, variant);
      return;
    }
    this._lines.update((lines) =>
      lines.map((l) => (l.product.id === product.id && l.variant.id === variant.id ? { ...l, qty } : l)),
    );
  }

  setExpedition(product: Product, variant: ProductVariant, courier: string): void {
    this._lines.update((lines) =>
      lines.map((l) => (l.product.id === product.id && l.variant.id === variant.id ? { ...l, expeditionCourier: courier } : l)),
    );
  }

  remove(product: Product, variant: ProductVariant): void {
    this._lines.update((lines) => lines.filter((l) => !(l.product.id === product.id && l.variant.id === variant.id)));
  }

  clear(): void {
    this._lines.set([]);
  }
}
