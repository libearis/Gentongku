import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';
import { AuthStore } from '../../../core/auth/auth-store.service';
import { CartStore } from '../data/cart-store.service';
import { ProductService } from '../data/product.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss',
})
export class ProductDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly cartStore = inject(CartStore);
  private readonly auth = inject(AuthStore);

  // Admin has read-only storefront access — no cart affordance for that role.
  readonly canPurchase = computed(() => this.auth.role() === 'Buyer' || this.auth.role() === 'Seller');

  readonly product = toSignal(
    this.route.paramMap.pipe(switchMap((params) => this.productService.get(params.get('id') ?? ''))),
    { initialValue: undefined },
  );

  readonly selectedVariantId = signal<string | null>(null);
  readonly qty = signal(1);
  readonly added = signal(false);

  readonly selectedVariant = computed(() => {
    const product = this.product();
    if (!product) return undefined;
    const id = this.selectedVariantId() ?? product.variants[0]?.id;
    return product.variants.find((v) => v.id === id) ?? product.variants[0];
  });

  selectVariant(id: string): void {
    this.selectedVariantId.set(id);
  }

  changeQty(delta: number): void {
    this.qty.update((q) => Math.max(1, q + delta));
  }

  addToCart(): void {
    const product = this.product();
    const variant = this.selectedVariant();
    if (!product || !variant) return;
    this.cartStore.add(product, variant, this.qty());
    this.added.set(true);
    setTimeout(() => this.added.set(false), 1500);
  }

  goToCart(): void {
    this.router.navigateByUrl('/storefront/cart');
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', maximumFractionDigits: 0 }).format(value);
  }
}
