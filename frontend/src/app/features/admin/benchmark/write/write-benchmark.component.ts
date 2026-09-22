import { DecimalPipe } from '@angular/common';
import { Component, signal } from '@angular/core';

interface WriteResult {
  table: 'orders_indexed' | 'orders_plain';
  rowsPerSec: number;
  avgMs: number;
}

@Component({
  selector: 'app-write-benchmark',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './write-benchmark.component.html',
  styleUrl: './write-benchmark.component.scss',
})
export class WriteBenchmarkComponent {
  readonly batchSize = signal(1000);
  readonly running = signal(false);
  readonly results = signal<WriteResult[] | null>(null);

  setBatchSize(value: string): void {
    const n = Number(value);
    this.batchSize.set(Number.isFinite(n) && n > 0 ? n : 1000);
  }

  /** Placeholder simulation — real numbers come from inserting the same batch into both tables server-side. */
  run(): void {
    this.running.set(true);
    setTimeout(() => {
      const n = this.batchSize();
      this.results.set([
        { table: 'orders_indexed', rowsPerSec: Math.round(n / 1.9), avgMs: 1.9 },
        { table: 'orders_plain', rowsPerSec: Math.round(n / 0.6), avgMs: 0.6 },
      ]);
      this.running.set(false);
    }, 500);
  }
}
