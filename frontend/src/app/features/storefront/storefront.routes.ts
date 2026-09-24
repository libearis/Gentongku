import { Routes } from '@angular/router';
import { buyerFlowGuard } from '../../core/auth/auth.guard';

// Shared by the Buyer route tree and the Seller "Shopping" tab — no duplicate storefront implementation.
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
    // Admin's storefront access is read-only, so cart/checkout are gated to Buyer/Seller despite the parent route allowing Admin.
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
  {
    path: 'wallet',
    canActivate: [buyerFlowGuard],
    loadComponent: () => import('./wallet/wallet.component').then((m) => m.WalletComponent),
  },
  { path: '', pathMatch: 'full', redirectTo: 'home' },
];
