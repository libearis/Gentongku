import { DecimalPipe } from '@angular/common';
import { Component, signal } from '@angular/core';

type Mode = 'rows' | 'size';

// Documented estimate constant per AGENTS.md section 7 — average row size, not byte-precise.
const AVG_ROW_BYTES = 420;

@Component({
  selector: 'app-generate-data',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './generate-data.component.html',
  styleUrl: './generate-data.component.scss',
})
export class GenerateDataComponent {
  readonly mode = signal<Mode>('rows');
  readonly rowCount = signal(100_000);
  readonly targetMb = signal(50);
  readonly submitting = signal(false);
  readonly justEnqueued = signal(false);

  readonly estimatedRowsFromSize = () => Math.round((this.targetMb() * 1024 * 1024) / AVG_ROW_BYTES);

  setMode(mode: Mode): void {
    this.mode.set(mode);
  }

  setRowCount(value: string): void {
    const n = Number(value);
    this.rowCount.set(Number.isFinite(n) && n > 0 ? n : 0);
  }

  setTargetMb(value: string): void {
    const n = Number(value);
    this.targetMb.set(Number.isFinite(n) && n > 0 ? n : 0);
  }

  /**
   * Fire-and-forget from the UI's perspective (AGENTS.md section 7): the
   * request should return success right away once the Scheduler API enqueues
   * the Hangfire job; the job's own lifecycle is tracked in Job Monitor.
   */
  enqueue(): void {
    this.submitting.set(true);
    setTimeout(() => {
      this.submitting.set(false);
      this.justEnqueued.set(true);
      setTimeout(() => this.justEnqueued.set(false), 2000);
    }, 400);
  }
}
