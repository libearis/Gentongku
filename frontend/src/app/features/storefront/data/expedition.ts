export interface ExpeditionOption {
  code: string;
  label: string;
  cost: number;
}

// Mirrors backend Ordering.Application/ExpeditionRates.cs — mocked flat rates, no real courier integration.
export const EXPEDITION_OPTIONS: ExpeditionOption[] = [
  { code: 'JNE', label: 'JNE', cost: 15_000 },
  { code: 'JNT', label: 'J&T', cost: 14_000 },
  { code: 'SiCepat', label: 'SiCepat', cost: 12_000 },
  { code: 'GoSend', label: 'GoSend', cost: 20_000 },
];
