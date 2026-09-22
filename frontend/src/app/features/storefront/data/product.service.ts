import { Injectable } from '@angular/core';
import { Observable, delay, of } from 'rxjs';
import { Product } from './product.model';

/**
 * Placeholder product catalog.
 *
 * The Catalog module isn't fully wired up on the backend yet, so this
 * service returns mocked data shaped exactly like the future HTTP
 * response. Swapping to the real API later is a one-file change: replace
 * the bodies below with `HttpClient` calls against
 * `${environment.apiUrl}/catalog/products` — the public method signatures
 * (`list()` / `get(id)`) and their `Observable<...>` return types stay the
 * same, so nothing calling this service needs to change.
 */
@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly products: Product[] = [
    {
      id: 'p1',
      name: 'Gentong Tanah Liat Klasik',
      category: 'Peralatan Rumah',
      price: 185000,
      description: 'Gentong tanah liat tradisional untuk menyimpan air minum agar tetap sejuk secara alami.',
      imageEmoji: '🏺',
      popular: true,
      sellerName: 'Kriya Tanah Mataram',
      variants: [
        { id: 'v1', label: '10 Liter', priceDelta: 0 },
        { id: 'v2', label: '20 Liter', priceDelta: 65000 },
      ],
    },
    {
      id: 'p2',
      name: 'Toples Keramik Bermotif',
      category: 'Dapur',
      price: 92000,
      description: 'Toples keramik dengan motif batik, cocok untuk menyimpan camilan kering.',
      imageEmoji: '🍯',
      popular: true,
      sellerName: 'Rumah Keramik Kasongan',
      variants: [
        { id: 'v1', label: 'Kecil', priceDelta: 0 },
        { id: 'v2', label: 'Besar', priceDelta: 40000 },
      ],
    },
    {
      id: 'p3',
      name: 'Kendi Air Minum',
      category: 'Peralatan Rumah',
      price: 65000,
      description: 'Kendi tanah liat klasik, menjaga air tetap dingin tanpa kulkas.',
      imageEmoji: '🫖',
      popular: false,
      sellerName: 'Kriya Tanah Mataram',
      variants: [{ id: 'v1', label: 'Standar', priceDelta: 0 }],
    },
    {
      id: 'p4',
      name: 'Pot Tanaman Gerabah',
      category: 'Taman',
      price: 45000,
      description: 'Pot gerabah berpori, baik untuk drainase akar tanaman hias.',
      imageEmoji: '🪴',
      popular: false,
      sellerName: 'System Seller',
      variants: [
        { id: 'v1', label: 'Diameter 15cm', priceDelta: 0 },
        { id: 'v2', label: 'Diameter 25cm', priceDelta: 30000 },
      ],
    },
    {
      id: 'p5',
      name: 'Cobek & Ulekan Batu',
      category: 'Dapur',
      price: 78000,
      description: 'Cobek batu andesit asli, permukaan kasar alami untuk hasil bumbu yang halus.',
      imageEmoji: '🥣',
      popular: true,
      sellerName: 'System Seller',
      variants: [{ id: 'v1', label: 'Standar', priceDelta: 0 }],
    },
    {
      id: 'p6',
      name: 'Celengan Gerabah',
      category: 'Dekorasi',
      price: 35000,
      description: 'Celengan gerabah bentuk klasik, dicat dengan pewarna alami.',
      imageEmoji: '🐖',
      popular: false,
      sellerName: 'Rumah Keramik Kasongan',
      variants: [{ id: 'v1', label: 'Standar', priceDelta: 0 }],
    },
  ];

  list(): Observable<Product[]> {
    return of(this.products).pipe(delay(150));
  }

  get(id: string): Observable<Product | undefined> {
    return of(this.products.find((p) => p.id === id)).pipe(delay(150));
  }
}
