import { apiClient } from './apiClient';
import { BaseResponse, ProductDto, ProductType } from '../types';

export const productService = {
  async getAllProducts(): Promise<BaseResponse<ProductDto[]>> {
    const response = await apiClient.get<BaseResponse<ProductDto[]>>('/products');
    return response.data;
  },

  async getProduct(id: number): Promise<BaseResponse<ProductDto>> {
    const response = await apiClient.get<BaseResponse<ProductDto>>(`/products/${id}`);
    return response.data;
  },

  async searchProducts(
    searchTerm?: string,
    type?: ProductType,
    franchiseId?: number
  ): Promise<BaseResponse<ProductDto[]>> {
    const params = new URLSearchParams();
    if (searchTerm) params.append('searchTerm', searchTerm);
    if (type !== undefined) params.append('type', type.toString());
    if (franchiseId) params.append('franchiseId', franchiseId.toString());

    const response = await apiClient.get<BaseResponse<ProductDto[]>>(
      `/products/search?${params.toString()}`
    );
    return response.data;
  },

  async getProductsByFranchise(franchiseId: number): Promise<BaseResponse<ProductDto[]>> {
    const response = await apiClient.get<BaseResponse<ProductDto[]>>(
      `/products/franchise/${franchiseId}`
    );
    return response.data;
  },

  async getProductsByType(type: ProductType): Promise<BaseResponse<ProductDto[]>> {
    const response = await apiClient.get<BaseResponse<ProductDto[]>>(
      `/products/type/${type}`
    );
    return response.data;
  },
};