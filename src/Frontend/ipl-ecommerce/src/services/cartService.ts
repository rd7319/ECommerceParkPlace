import { apiClient } from './apiClient';
import { BaseResponse, CartDto, AddToCartRequest, UpdateCartItemRequest, OrderDto,CreateOrderRequest } from '../types';

export const cartService = {
  async getUserCart(userId: number): Promise<BaseResponse<CartDto>> {
    const response = await apiClient.get<BaseResponse<CartDto>>(`/cart/user/${userId}`);
    return response.data;
  },

  async addToCart(request: AddToCartRequest): Promise<BaseResponse<CartDto>> {
    const response = await apiClient.post<BaseResponse<CartDto>>('/cart/add', request);
    return response.data;
  },

  async updateCartItem(request: UpdateCartItemRequest): Promise<BaseResponse<CartDto>> {
    const response = await apiClient.put<BaseResponse<CartDto>>('/cart/update', request);
    return response.data;
  },

  async removeFromCart(userId: number, productId: number): Promise<BaseResponse<CartDto>> {
    const response = await apiClient.delete<BaseResponse<CartDto>>(
      `/cart/remove/${userId}/${productId}`
    );
    return response.data;
  },

  async clearCart(userId: number): Promise<BaseResponse<CartDto>> {
    const response = await apiClient.delete<BaseResponse<CartDto>>(`/cart/clear/${userId}`);
    return response.data;
  },
};