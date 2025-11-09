import { apiClient } from './apiClient';
import { BaseResponse, OrderDto, CreateOrderRequest } from '../types';

export const orderService = {
  async getUserOrders(userId: number): Promise<BaseResponse<OrderDto[]>> {
    const response = await apiClient.get<BaseResponse<OrderDto[]>>(`/orders/user/${userId}`);
    return response.data;
  },

  async getOrder(id: number): Promise<BaseResponse<OrderDto>> {
    const response = await apiClient.get<BaseResponse<OrderDto>>(`/orders/${id}`);
    return response.data;
  },

  async createOrder(request: CreateOrderRequest): Promise<BaseResponse<OrderDto>> {
    const response = await apiClient.post<BaseResponse<OrderDto>>('/orders/create', request);
    return response.data;
  },
};