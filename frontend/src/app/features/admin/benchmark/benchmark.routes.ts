import { Routes } from '@angular/router';

// Search (Elasticsearch) isn't routed here — it's phase 2, shown as a disabled sidebar entry instead (AGENTS.md 6.3).
export const BENCHMARK_ROUTES: Routes = [
  {
    path: 'read',
    loadComponent: () => import('./read/read-benchmark.component').then((m) => m.ReadBenchmarkComponent),
  },
  {
    path: 'write',
    loadComponent: () => import('./write/write-benchmark.component').then((m) => m.WriteBenchmarkComponent),
  },
  {
    path: 'health',
    loadComponent: () => import('./health/health-check.component').then((m) => m.HealthCheckComponent),
  },
  { path: '', pathMatch: 'full', redirectTo: 'read' },
];
