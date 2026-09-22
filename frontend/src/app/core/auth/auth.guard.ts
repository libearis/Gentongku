import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from './auth-store.service';
import { Role } from './auth.models';

/**
 * Role → route enforcement lives here, not in components/templates
 * (AGENTS.md section 12). Exact mapping per AGENTS.md section 4:
 *   Admin  -> /admin/**            + read-only /storefront/**
 *   Buyer  -> /storefront/** only
 *   Seller -> /seller/my-store/**  + /storefront/** (under "Shopping")
 */

/** Requires any authenticated session; otherwise redirect to /auth. */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (auth.isAuthenticated()) {
    return true;
  }
  return router.parseUrl('/auth');
};

/** Requires the session's role to be one of `roles`, else send the user to their own home route. */
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

/** Storefront routes that mutate state (cart/checkout) — Admin gets read-only access, so it's excluded here. */
export const buyerFlowGuard: CanActivateFn = roleGuard(['Buyer', 'Seller']);

export const adminGuard: CanActivateFn = roleGuard(['Admin']);
export const sellerGuard: CanActivateFn = roleGuard(['Seller']);
export const storefrontGuard: CanActivateFn = roleGuard(['Admin', 'Buyer', 'Seller']);

/** Keeps an already-authenticated user off the /auth screen. */
export const guestGuard: CanActivateFn = () => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (!auth.isAuthenticated()) {
    return true;
  }
  return router.parseUrl(auth.homeRouteFor(auth.role() ?? 'Buyer'));
};

/** Used on the wildcard route: never activates, only redirects to /auth or the caller's own role home. */
export const catchAllGuard: CanActivateFn = () => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (!auth.isAuthenticated()) {
    return router.parseUrl('/auth');
  }
  return router.parseUrl(auth.homeRouteFor(auth.role() ?? 'Buyer'));
};
