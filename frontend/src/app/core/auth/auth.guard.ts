import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from './auth-store.service';
import { Role } from './auth.models';

// Role -> allowed routes: Admin = /admin/** + read-only /storefront/**, Buyer = /storefront/** only, Seller = /seller/my-store/** + /storefront/**.
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (auth.isAuthenticated()) {
    return true;
  }
  return router.parseUrl('/auth');
};

export function roleGuard(roles: Role[]): CanActivateFn {
  return () => {
    const auth = inject(AuthStore);
    const router = inject(Router);

    if (!auth.isAuthenticated()) {
      return router.parseUrl('/auth');
    }
    const role = auth.role();
    if (role && roles.includes(role)) {
      return true;
    }
    return router.parseUrl(auth.homeRouteFor(role ?? 'Buyer'));
  };
}

// Admin is deliberately excluded: it gets read-only storefront access, so it can't reach cart/checkout mutation routes.
export const buyerFlowGuard: CanActivateFn = roleGuard(['Buyer', 'Seller']);

export const adminGuard: CanActivateFn = roleGuard(['Admin']);
export const sellerGuard: CanActivateFn = roleGuard(['Seller']);
export const storefrontGuard: CanActivateFn = roleGuard(['Admin', 'Buyer', 'Seller']);

export const guestGuard: CanActivateFn = () => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (!auth.isAuthenticated()) {
    return true;
  }
  return router.parseUrl(auth.homeRouteFor(auth.role() ?? 'Buyer'));
};

// Wildcard-route guard: never activates, only redirects to /auth or the caller's own role home.
export const catchAllGuard: CanActivateFn = () => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (!auth.isAuthenticated()) {
    return router.parseUrl('/auth');
  }
  return router.parseUrl(auth.homeRouteFor(auth.role() ?? 'Buyer'));
};
