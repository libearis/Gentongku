import { Component, signal } from '@angular/core';

interface HealthItem {
  name: string;
  status: 'Healthy' | 'Degraded' | 'Unhealthy';
}

@Component({
  selector: 'app-health-check',
  standalone: true,
  templateUrl: './health-check.component.html',
  styleUrl: './health-check.component.scss',
})
export class HealthCheckComponent {
  // Placeholder — real data comes from ASP.NET Core health checks at /health.
  readonly items = signal<HealthItem[]>([
    { name: 'PostgreSQL', status: 'Healthy' },
    { name: 'Redis', status: 'Healthy' },
    { name: 'Hangfire storage', status: 'Healthy' },
  ]);
}
