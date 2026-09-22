import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthStore } from '../../../core/auth/auth-store.service';
import { WordmarkComponent } from '../../ui/wordmark/wordmark.component';

/**
 * AdminShell: dark/technical register sidebar + router-outlet, no business
 * logic (AGENTS.md section 11). Applies `.theme-dark` to switch the design
 * tokens for Admin/Benchmark/Scheduler.
 */
@Component({
  selector: 'app-admin-shell',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet, WordmarkComponent],
  templateUrl: './admin-shell.component.html',
  styleUrl: './admin-shell.component.scss',
  host: { class: 'theme-dark' },
})
export class AdminShellComponent {
  readonly auth = inject(AuthStore);

  logout(): void {
    this.auth.logout();
  }
}
