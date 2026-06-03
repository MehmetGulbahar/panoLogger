import { apiClient } from './client';
export async function request(config) {
    const response = await apiClient.request(config);
    return response.data;
}
