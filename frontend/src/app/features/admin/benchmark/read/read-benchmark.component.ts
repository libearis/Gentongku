import { Component, computed, signal } from '@angular/core';

type TableName = 'Order' | 'Produk' | 'Kategori';

interface ColumnInfo {
  name: string;
  indexed: boolean;
}

const TABLE_COLUMNS: Record<TableName, ColumnInfo[]> = {
  Order: [
    { name: 'status', indexed: true },
    { name: 'created_at', indexed: true },
    { name: 'user_id', indexed: true },
    { name: 'notes', indexed: false },
  ],
  Produk: [
    { name: 'name', indexed: false },
    { name: 'category_id', indexed: true },
    { name: 'seller_id', indexed: true },
  ],
  Kategori: [
    { name: 'name', indexed: true },
    { name: 'parent_id', indexed: true },
  ],
};

const TABLE_SIZES: Record<TableName, string> = {
  Order: '~2.4M rows',
  Produk: '~86k rows',
  Kategori: '~42 rows',
};

interface StrategyResult {
  key: 'redis' | 'index' | 'live' | 'indexRedis';
  label: string;
  enabled: boolean;
  ms: number | null;
}

@Component({
  selector: 'app-read-benchmark',
  standalone: true,
  templateUrl: './read-benchmark.component.html',
  styleUrl: './read-benchmark.component.scss',
})
export class ReadBenchmarkComponent {
  readonly tables: TableName[] = ['Order', 'Produk', 'Kategori'];
  readonly tableSizes = TABLE_SIZES;

  readonly selectedTable = signal<TableName>('Order');
  readonly columns = computed(() => TABLE_COLUMNS[this.selectedTable()]);
  readonly selectedColumn = signal<string>(TABLE_COLUMNS['Order'][0].name);

  readonly strategies = signal<StrategyResult[]>([
    { key: 'redis', label: 'Redis only', enabled: true, ms: null },
    { key: 'index', label: 'DB index only', enabled: true, ms: null },
    { key: 'live', label: 'Live query (full scan)', enabled: true, ms: null },
    { key: 'indexRedis', label: 'Index + Redis', enabled: true, ms: null },
  ]);

  readonly running = signal(false);

  selectTable(table: TableName): void {
    this.selectedTable.set(table);
    this.selectedColumn.set(TABLE_COLUMNS[table][0].name);
  }

  toggleStrategy(key: StrategyResult['key']): void {
    this.strategies.update((list) => list.map((s) => (s.key === key ? { ...s, enabled: !s.enabled } : s)));
  }

  isColumnIndexed(): boolean {
    return this.columns().find((c) => c.name === this.selectedColumn())?.indexed ?? false;
  }

  // Simulation only: models the real degradation rule where a non-indexed column drops index-only/index+redis to live-query/redis-only.
  runBenchmark(): void {
    this.running.set(true);
    const indexed = this.isColumnIndexed();

    setTimeout(() => {
      this.strategies.update((list) =>
        list.map((s) => {
          if (!s.enabled) return { ...s, ms: null };
          const base = { redis: 3, index: indexed ? 18 : 420, live: 420, indexRedis: indexed ? 3 : 3 }[s.key];
          const jitter = Math.round(base * (0.85 + Math.random() * 0.3));
          return { ...s, ms: jitter };
        }),
      );
      this.running.set(false);
    }, 500);
  }
}
