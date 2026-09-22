export type OrderStatus = 'Pending' | 'Paid' | 'Shipped' | 'Completed';
export type PaymentMethod = 'transfer' | 'qris' | 'cod';

export interface OrderItem {
  productName: string;
  variantLabel: string;
  qty: number;
  price: number;
}

export interface Order {
  id: string;
  placedAt: string;
  status: OrderStatus;
  paymentMethod: PaymentMethod;
  items: OrderItem[];
  total: number;
}
