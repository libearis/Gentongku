import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, AuthUser, LoginRequest, RegisterRequest, Role } from './auth.models';

/**
 * AuthStore — the single source of truth for the current session.
 *
 * Signal-based, analogous to a Pinia store: components read `user()`,
 * `role()`, `isAuthenticated()` via computed signals and never duplicate
 * role logic themselves (AGENTS.md section 11/12).
 *
 * Trade-off (documented per AGENTS.md section 4): the JWT is kept in an
 * in-memory signal only, never in localStorage. This means a full page
 * reload loses the session (no silent refresh-on-reload), which is an
 * accepted trade-off for this portfolio project in exchange for not
 * exposing the raw token to XSS-readable storage. A production app would
 * pair this with an httpOnly refresh-token cookie; that's out of scope here.
 */
@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly http = inject(HttpClient);

  private readonly _token = signal<string | null>(null);
  private readonly _user = signal<AuthUser | null>(null);

  readonly token = this._token.asReadonly();
  readonly user = this._user.asReadonly();

  readonly isAuthenticated = computed(() => this._token() !== null);
  readonly role = computed<Role | null>(() => this._user()?.role ?? null);

  readonly isAdmin = computed(() => this.role() === 'Admin');
  readonly isBuyer = computed(() => this.role() === 'Buyer');
  readonly isSeller = computed(() => this.role() === 'Seller');

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, payload)
      .pipe(tap((res) => this.setSession(res)));
  }

  register(payload: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, payload)
      .pipe(tap((res) => this.setSession(res)));
  }

  logout(): void {
    this._token.set(null);
    this._user.set(null);
  }

  /** Home route to land on right after auth, based on role. */
  homeRouteFor(role: Role): string {
    switch (role) {
      case 'Admin':
        return '/admin/users';
      case 'Seller':
        return '/seller/my-store';
      case 'Buyer':
      default:
        return '/storefront/home';
    }
  }

  private setSession(res: AuthResponse): void {
    this._token.set(res.token);
    this._user.set({
      id: res.userId ?? '',
      username: res.username ?? '',
      email: res.email ?? '',
      role: res.role,
      storeName: res.storeName,
    });
  }
}
