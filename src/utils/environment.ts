export function getApiBaseUrl(): string {
  return import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5195/api';
}

export function getAppName(): string {
  return import.meta.env.VITE_APP_NAME ?? 'Electrical Panel Documentation & QR Management System';
}
