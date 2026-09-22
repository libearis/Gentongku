import { Component, signal } from '@angular/core';

interface AdminUserRow {
  email: string;
  role: 'Admin' | 'Buyer' | 'Seller';
  createdAt: string;
}

@Component({
  selector: 'app-users-page',
  standalone: true,
  templateUrl: './users-page.component.html',
  styleUrl: './users-page.component.scss',
})
export class UsersPageComponent {
  // Placeholder rows — Identity module admin-listing endpoint isn't wired up yet.
  readonly users = signal<AdminUserRow[]>([
    { email: 'admin@gentongku.dev', role: 'Admin', createdAt: '2026-08-01' },
    { email: 'buyer1@example.com', role: 'Buyer', createdAt: '2026-08-14' },
    { email: 'seller1@example.com', role: 'Seller', createdAt: '2026-08-20' },
  ]);

  readonly showCreateAdmin = signal(false);

  toggleCreateAdmin(): void {
    this.showCreateAdmin.update((v) => !v);
  }
}
