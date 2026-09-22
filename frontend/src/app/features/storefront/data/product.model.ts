export interface ProductVariant {
  id: string;
  label: string;
  priceDelta: number;
}

export interface Product {
  id: string;
  name: string;
  category: string;
  price: number;
  description: string;
  imageEmoji: string;
  popular: boolean;
  sellerName: string;
  variants: ProductVariant[];
}
