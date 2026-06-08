export function getApiBaseUrl(): string {
  if (import.meta.env.VITE_API_BASE_URL) {
    return import.meta.env.VITE_API_BASE_URL;
  }

  if (typeof window !== 'undefined') {
    const protocol = window.location.protocol;
    const hostname = window.location.hostname;

    if (protocol === 'https:' && !['localhost', '127.0.0.1'].includes(hostname)) {
      return 'https://panologger.onrender.com/api';
    }

    return `${protocol}//${hostname}:5195/api`;
  }

  return 'http://localhost:5195/api';
}

export function getAppName(): string {
  return import.meta.env.VITE_APP_NAME ?? 'Electrical Panel Documentation & QR Management System';
}
