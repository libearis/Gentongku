import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { AuthStore } from '../../../core/auth/auth-store.service';
import { Role } from '../../../core/auth/auth.models';
import { WordmarkComponent } from '../../../shared/ui/wordmark/wordmark.component';

type Tab = 'login' | 'register';

@Component({
  selector: 'app-auth-page',
  standalone: true,
  imports: [ReactiveFormsModule, WordmarkComponent],
  templateUrl: './auth-page.component.html',
  styleUrl: './auth-page.component.scss',
})
export class AuthPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthStore);
  private readonly router = inject(Router);

  readonly tab = signal<Tab>('login');
  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly registerRole = signal<Extract<Role, 'Buyer' | 'Seller'>>('Buyer');

  readonly loginForm = this.fb.nonNullable.group({
    identifier: ['', [Validators.required, Validators.minLength(3)]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  readonly registerForm = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.minLength(3), Validators.pattern(/^[a-zA-Z0-9_.]+$/)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    storeName: [''],
  });

  setTab(tab: Tab): void {
    this.tab.set(tab);
    this.errorMessage.set(null);
  }

  setRegisterRole(role: Extract<Role, 'Buyer' | 'Seller'>): void {
    this.registerRole.set(role);
    const storeNameCtrl = this.registerForm.controls.storeName;
    if (role === 'Seller') {
      storeNameCtrl.addValidators([Validators.required]);
    } else {
      storeNameCtrl.clearValidators();
    }
    storeNameCtrl.updateValueAndValidity();
  }

  submitLogin(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    this.submitting.set(true);
    this.errorMessage.set(null);
    this.auth
      .login(this.loginForm.getRawValue())
      .pipe(
        catchError((err) => {
          this.errorMessage.set(err?.error?.message ?? 'Login failed. Check your credentials and try again.');
          return of(null);
        }),
      )
      .subscribe((res) => {
        this.submitting.set(false);
        if (res) {
          this.router.navigateByUrl(this.auth.homeRouteFor(res.role));
        }
      });
  }

  submitRegister(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.submitting.set(true);
    this.errorMessage.set(null);
    const { username, email, password, storeName } = this.registerForm.getRawValue();
    const role = this.registerRole();

    this.auth
      .register({
        username,
        email,
        password,
        role,
        ...(role === 'Seller' ? { storeName } : {}),
      })
      .pipe(
        catchError((err) => {
          this.errorMessage.set(err?.error?.message ?? 'Registration failed. Please try again.');
          return of(null);
        }),
      )
      .subscribe((res) => {
        this.submitting.set(false);
        if (res) {
          this.router.navigateByUrl(this.auth.homeRouteFor(res.role));
        }
      });
  }
}
