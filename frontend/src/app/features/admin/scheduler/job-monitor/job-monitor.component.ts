import { Component, signal } from '@angular/core';

interface JobRun {
  jobId: string;
  type: string;
  status: 'Processing' | 'Success' | 'Error';
  startedAt: string;
}

@Component({
  selector: 'app-job-monitor',
  standalone: true,
  templateUrl: './job-monitor.component.html',
  styleUrl: './job-monitor.component.scss',
})
export class JobMonitorComponent {
  // Placeholder rows — real data polls a job_runs table or Hangfire storage directly.
  readonly jobs = signal<JobRun[]>([
    { jobId: 'a1f9c2', type: 'GenerateDummyData', status: 'Success', startedAt: '2026-09-21 22:14' },
    { jobId: 'b7e031', type: 'GenerateDummyData', status: 'Processing', startedAt: '2026-09-22 09:02' },
  ]);
}
