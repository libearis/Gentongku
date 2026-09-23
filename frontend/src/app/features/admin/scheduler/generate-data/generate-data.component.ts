import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DummyDataTable, SchedulerService } from '../data/scheduler.service';

type Mode = 'rows' | 'size';

// Documented estimate constant per AGENTS.md section 7 — average row size, not byte-precise.
const AVG_ROW_BYTES = 420;

interface TableOption {
  value: DummyDataTable;
  label: string;
  description: string;
}

const TABLE_OPTIONS: TableOption[] = [
  {
    value: 'Category',
    label: 'Kategori',
    description: 'Menambah kategori produk baru dengan nama acak.',
  },
  {
    value: 'Product',
    label: 'Produk',
    description: 'Menambah produk baru, masing-masing dikaitkan ke kategori & penjual yang sudah ada secara acak.',
  },
  {
    value: 'Order',
    label: 'Pesanan',
    description: 'Menambah pesanan baru, masing-masing memilih pembeli & 1-3 produk yang sudah ada secara acak untuk menghitung totalnya.',
  },
];

@Component({
  selector: 'app-generate-data',
  standalone: true,
  imports: [DecimalPipe, RouterLink],
  templateUrl: './generate-data.component.html',
  styleUrl: './generate-data.component.scss',
})
export class GenerateDataComponent {
  private readonly schedulerService = inject(SchedulerService);

  readonly tableOptions = TABLE_OPTIONS;
  readonly selectedTable = signal<DummyDataTable>('Product');
  readonly selectedTableInfo = computed(() => this.tableOptions.find((t) => t.value === this.selectedTable())!);

  readonly mode = signal<Mode>('rows');
  readonly rowCount = signal(1_000);
  readonly targetMb = signal(5);
  readonly submitting = signal(false);
  readonly justEnqueued = signal(false);
  readonly error = signal<string | null>(null);

  readonly estimatedRowsFromSize = () => Math.round((this.targetMb() * 1024 * 1024) / AVG_ROW_BYTES);

  selectTable(table: DummyDataTable): void {
    this.selectedTable.set(table);
  }

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

  // Fire-and-forget: success means the job was enqueued, not that it finished — track its lifecycle in Job Monitor (AGENTS.md 7).
  enqueue(): void {
    this.submitting.set(true);
    this.error.set(null);

    this.schedulerService
      .generateDummyData({
        table: this.selectedTable(),
        targetRowCount: this.mode() === 'rows' ? this.rowCount() : undefined,
        targetStorageBytes: this.mode() === 'size' ? this.targetMb() * 1024 * 1024 : undefined,
      })
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.justEnqueued.set(true);
          setTimeout(() => this.justEnqueued.set(false), 3000);
        },
        error: () => {
          this.submitting.set(false);
          this.error.set('Gagal mengirim permintaan. Pastikan backend berjalan, lalu coba lagi.');
        },
      });
  }
}
