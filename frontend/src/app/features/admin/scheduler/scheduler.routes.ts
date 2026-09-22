import { Routes } from '@angular/router';

export const SCHEDULER_ROUTES: Routes = [
  {
    path: 'generate-data',
    loadComponent: () => import('./generate-data/generate-data.component').then((m) => m.GenerateDataComponent),
  },
  {
    path: 'job-monitor',
    loadComponent: () => import('./job-monitor/job-monitor.component').then((m) => m.JobMonitorComponent),
  },
  { path: '', pathMatch: 'full', redirectTo: 'generate-data' },
];
