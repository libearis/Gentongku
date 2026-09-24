export type OrderStatus = 'Pending' | 'Paid' | 'Shipped' | 'Completed';

export interface OrderItem {
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  expeditionCourier: string;
  expeditionCost: number;
}

export interface Order {
  id: string;
  buyerId: string;
  sellerId: string;
  checkoutGroupId: string;
  totalAmount: number;
  status: OrderStatus;
  notes: string | null;
  createdAt: string;
  items: OrderItem[];
}

export interface CheckoutItem {
  productId: string;
  quantity: number;
  expeditionCourier: string;
}
