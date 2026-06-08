export function getApiBaseUrl() {
    if (import.meta.env.VITE_API_BASE_URL) {
        return import.meta.env.VITE_API_BASE_URL;
    }
    if (typeof window !== 'undefined') {
        const protocol = window.location.protocol;
        const hostname = window.location.hostname;
        return `${protocol}//${hostname}:5195/api`;
    }
    return 'http://localhost:5195/api';
}
export function getAppName() {
    return import.meta.env.VITE_APP_NAME ?? 'Electrical Panel Documentation & QR Management System';
}
