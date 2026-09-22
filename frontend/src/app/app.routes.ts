import { Routes } from '@angular/router';
import { adminGuard, catchAllGuard, guestGuard, sellerGuard, storefrontGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'auth',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/auth-page/auth-page.component').then((m) => m.AuthPageComponent),
  },
  {
    path: 'storefront',
    canActivate: [storefrontGuard],
    loadComponent: () =>
      import('./shared/layout/buyer-shell/buyer-shell.component').then((m) => m.BuyerShellComponent),
    loadChildren: () => import('./features/storefront/storefront.routes').then((m) => m.STOREFRONT_ROUTES),
  },
  {
    path: 'seller',
    canActivate: [sellerGuard],
    loadComponent: () =>
      import('./shared/layout/seller-shell/seller-shell.component').then((m) => m.SellerShellComponent),
    loadChildren: () => import('./features/seller/seller.routes').then((m) => m.SELLER_ROUTES),
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./shared/layout/admin-shell/admin-shell.component').then((m) => m.AdminShellComponent),
    loadChildren: () => import('./features/admin/admin.routes').then((m) => m.ADMIN_ROUTES),
  },
  { path: '', pathMatch: 'full', redirectTo: 'auth' },
  { path: '**', canActivate: [catchAllGuard], children: [] },
];
