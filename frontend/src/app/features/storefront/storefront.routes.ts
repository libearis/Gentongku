import { Routes } from '@angular/router';
import { buyerFlowGuard } from '../../core/auth/auth.guard';

/**
 * Shared by both the Buyer route tree (/storefront/**) and the Seller
 * "Shopping" tab, which links straight into these same routes — no
 * duplicate storefront implementation (AGENTS.md section 4/5).
 */
export const STOREFRONT_ROUTES: Routes = [
  {
    path: 'home',
    loadComponent: () => import('./home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'product/:id',
    loadComponent: () =>
      import('./product-detail/product-detail.component').then((m) => m.ProductDetailComponent),
  },
  {
    // Cart/checkout mutate state — Admin's storefront access is read-only, so these are
    // additionally gated to Buyer/Seller only, even though the parent route already allows Admin through.
    path: 'cart',
    canActivate: [buyerFlowGuard],
    loadComponent: () => import('./cart/cart.component').then((m) => m.CartComponent),
  },
  {
    path: 'checkout',
    canActivate: [buyerFlowGuard],
    loadComponent: () => import('./checkout/checkout.component').then((m) => m.CheckoutComponent),
  },
  {
    path: 'orders',
    canActivate: [buyerFlowGuard],
    loadComponent: () =>
      import('./order-history/order-history.component').then((m) => m.OrderHistoryComponent),
  },
  { path: '', pathMatch: 'full', redirectTo: 'home' },
];
