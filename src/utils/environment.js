export function getApiBaseUrl() {
    return import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5195/api';
}
export function getAppName() {
    return import.meta.env.VITE_APP_NAME ?? 'Electrical Panel Documentation & QR Management System';
}
