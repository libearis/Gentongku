import { Routes } from '@angular/router';

/**
 * Search (Elasticsearch) is intentionally NOT routed here — it's phase 2
 * (AGENTS.md section 6.3). The sidebar shows a disabled nav entry with a
 * "Fase 2" badge instead of a clickable link (see AdminShellComponent).
 */
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
