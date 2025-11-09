import { apiClient } from './apiClient';
import { BaseResponse, FranchiseDto } from '../types';

export const franchiseService = {
  async getAllFranchises(): Promise<BaseResponse<FranchiseDto[]>> {
    const response = await apiClient.get<BaseResponse<FranchiseDto[]>>('/franchises');
    return response.data;
  },

  async getFranchise(id: number): Promise<BaseResponse<FranchiseDto>> {
    const response = await apiClient.get<BaseResponse<FranchiseDto>>(`/franchises/${id}`);
    return response.data;
  },
};