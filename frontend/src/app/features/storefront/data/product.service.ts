import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Product } from './product.model';

/** Shape returned by the Catalog module's `GET /api/catalog/products[/:id]` (ProductDto). */
interface ApiProductDto {
  id: string;
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  categoryId: string;
  categoryName: string;
  sellerId: string;
  sellerName: string;
  isActive: boolean;
}

const CATEGORY_EMOJI: Record<string, string> = {
  'Peralatan Rumah': '🏺',
  Dapur: '🍯',
  Taman: '🪴',
  Dekorasi: '🐖',
};

/**
 * Reads real Catalog data from the backend (AGENTS.md section 3's Catalog
 * module). The backend doesn't model product variants yet, so each product
 * is given a single "Standar" variant so the existing cart/checkout flow
 * (which is keyed on product + variant) keeps working unchanged.
 */
@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);

  list(): Observable<Product[]> {
    return this.http
      .get<ApiProductDto[]>(`${environment.apiUrl}/catalog/products`, { params: { take: 50 } })
      .pipe(map((products) => products.map(toProduct)));
  }

  get(id: string): Observable<Product | undefined> {
    return this.http.get<ApiProductDto>(`${environment.apiUrl}/catalog/products/${id}`).pipe(
      map(toProduct),
      catchError(() => of(undefined)),
    );
  }
}

function toProduct(dto: ApiProductDto): Product {
  return {
    id: dto.id,
    name: dto.name,
    category: dto.categoryName,
    price: dto.price,
    description: dto.description ?? '',
    imageEmoji: CATEGORY_EMOJI[dto.categoryName] ?? '📦',
    popular: false,
    sellerName: dto.sellerName,
    variants: [{ id: 'standar', label: 'Standar', priceDelta: 0 }],
  };
}
