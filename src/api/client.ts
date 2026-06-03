import axios from 'axios';

import { getApiBaseUrl } from '@/utils/environment';

export const apiClient = axios.create({
  baseURL: getApiBaseUrl(),
  timeout: 30000,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});
