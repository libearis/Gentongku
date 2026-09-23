import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { AuthStore } from '../../../core/auth/auth-store.service';
import { WordmarkComponent } from '../../ui/wordmark/wordmark.component';

interface AdminNavItem {
  label: string;
  route?: string;
  disabled?: boolean;
  badge?: string;
}

interface AdminNavSection {
  key: string;
  label: string;
  items: AdminNavItem[];
}

const NAV_SECTIONS: AdminNavSection[] = [
  {
    key: 'admin-menu',
    label: 'Admin Menu',
    items: [
      { label: 'Users', route: '/admin/users' },
      { label: 'Ticketing', route: '/admin/ticketing' },
    ],
  },
  {
    key: 'benchmark',
    label: 'Benchmark',
    items: [
      { label: 'Read', route: '/admin/benchmark/read' },
      { label: 'Write', route: '/admin/benchmark/write' },
      { label: 'Search (Elasticsearch)', disabled: true, badge: 'Fase 2' },
      { label: 'Health', route: '/admin/benchmark/health' },
    ],
  },
  {
    key: 'scheduler',
    label: 'Scheduler',
    items: [
      { label: 'Generate Data', route: '/admin/scheduler/generate-data' },
      { label: 'Job Monitor', route: '/admin/scheduler/job-monitor' },
    ],
  },
];

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
  private readonly router = inject(Router);

  readonly search = signal('');
  private readonly collapsed = signal(new Set<string>(NAV_SECTIONS.map((s) => s.key)));

  constructor() {
    // Search is transient nav aid, not page state — clear it on every navigation, including back/forward.
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntilDestroyed(),
      )
      .subscribe(() => this.search.set(''));
  }

  readonly sections = computed(() => {
    const term = this.search().trim().toLowerCase();
    const collapsed = this.collapsed();

    return NAV_SECTIONS.map((section) => {
      const items = term
        ? section.items.filter((item) => item.label.toLowerCase().includes(term))
        : section.items;

      return {
        ...section,
        items,
        expanded: term.length > 0 ? items.length > 0 : !collapsed.has(section.key),
      };
    }).filter((section) => term.length === 0 || section.items.length > 0);
  });

  toggleSection(key: string): void {
    this.collapsed.update((current) => {
      const next = new Set(current);
      if (next.has(key)) next.delete(key);
      else next.add(key);
      return next;
    });
  }

  logout(): void {
    this.auth.logout();
  }
}
