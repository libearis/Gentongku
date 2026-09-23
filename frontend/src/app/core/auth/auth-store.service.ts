import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, AuthUser, LoginRequest, RegisterRequest, Role } from './auth.models';

// JWT is kept only in this in-memory signal (never localStorage), so a page reload loses the session by design, trading silent refresh for XSS safety.
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
