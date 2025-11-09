export enum ProductType {
  Jersey = 1,
  Cap = 2,
  Flag = 3,
  AutographedPhoto = 4,
  Poster = 5,
  Keychain = 6,
  Mug = 7,
  TShirt = 8,
}

export enum OrderStatus {
  Pending = 1,
  Processing = 2,
  Shipped = 3,
  Delivered = 4,
  Cancelled = 5,
}

export interface BaseResponse<T> {
  isSuccess: boolean;
  message?: string;
  data?: T;
  errors: string[];
}

export interface FranchiseDto {
  id: number;
  name: string;
  shortName: string;
  city: string;
  color: string;
  logoUrl: string;
  description: string;
}

export interface ProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  type: ProductType;
  typeName: string;
  imageUrl: string;
  stockQuantity: number;
  isAvailable: boolean;
  size: string;
  color: string;
  franchiseId: number;
  franchise: FranchiseDto;
}

export interface CartItemDto {
  id: number;
  productId: number;
  quantity: number;
  product: ProductDto;
  itemTotal: number;
}

export interface CartDto {
  id: number;
  userId: number;
  cartItems: CartItemDto[];
  totalAmount: number;
}

export interface OrderItemDto {
  id: number;
  productId: number;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  product: ProductDto;
}

export interface OrderDto {
  id: number;
  userId: number;
  orderNumber: string;
  totalAmount: number;
  status: OrderStatus;
  statusName: string;
  orderDate: string;
  shippingAddress: string;
  trackingNumber?: string;
  orderItems: OrderItemDto[];
}

export interface AddToCartRequest {
  userId: number;
  productId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  userId: number;
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  userId: number;
  shippingAddress: string;
}