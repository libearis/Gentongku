import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';

export type DummyDataTable = 'Category' | 'Product' | 'Order';

export interface GenerateDummyDataRequest {
  table: DummyDataTable;
  targetRowCount?: number;
  targetStorageBytes?: number;
}

/** Matches Scheduler.Application.DTOs.JobRunDto. */
export interface JobRun {
  id: string;
  hangfireJobId: string;
  jobType: string;
  status: 'Processing' | 'Success' | 'Error';
  resultMessage: string | null;
  createdAt: string;
  completedAt: string | null;
}

@Injectable({ providedIn: 'root' })
export class SchedulerService {
  private readonly http = inject(HttpClient);

  generateDummyData(request: GenerateDummyDataRequest): Observable<{ hangfireJobId: string }> {
    return this.http.post<{ hangfireJobId: string }>(`${environment.apiUrl}/scheduler/generate-dummy-data`, request);
  }

  listJobs(): Observable<JobRun[]> {
    return this.http.get<JobRun[]>(`${environment.apiUrl}/scheduler/jobs`);
  }
}
