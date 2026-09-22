import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: 'users',
    loadComponent: () => import('./users/users-page.component').then((m) => m.UsersPageComponent),
  },
  {
    path: 'ticketing',
    loadComponent: () => import('./ticketing/ticketing-page.component').then((m) => m.TicketingPageComponent),
  },
  {
    path: 'benchmark',
    loadChildren: () => import('./benchmark/benchmark.routes').then((m) => m.BENCHMARK_ROUTES),
  },
  {
    path: 'scheduler',
    loadChildren: () => import('./scheduler/scheduler.routes').then((m) => m.SCHEDULER_ROUTES),
  },
  { path: '', pathMatch: 'full', redirectTo: 'users' },
];
