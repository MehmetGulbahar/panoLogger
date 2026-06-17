const CACHE_VERSION = 'panoveri-v1';
const STATIC_CACHE = `${CACHE_VERSION}-static`;
const RUNTIME_CACHE = `${CACHE_VERSION}-runtime`;

const STATIC_ASSETS = [
  '/',
  '/offline.html',
  '/manifest.json',
  '/icons/icon-72x72.png',
  '/icons/icon-96x96.png',
  '/icons/icon-128x128.png',
  '/icons/icon-144x144.png',
  '/icons/icon-152x152.png',
  '/icons/icon-192x192.png',
  '/icons/icon-384x384.png',
  '/icons/icon-512x512.png'
];

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches
      .open(STATIC_CACHE)
      .then((cache) => cache.addAll(STATIC_ASSETS))
  );
});

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches
      .keys()
      .then((keys) =>
        Promise.all(
          keys
            .filter((key) => ![STATIC_CACHE, RUNTIME_CACHE].includes(key))
            .map((key) => caches.delete(key))
        )
      )
      .then(() => self.clients.claim())
  );
});

self.addEventListener('message', (event) => {
  if (event.data?.type === 'SKIP_WAITING') {
    self.skipWaiting();
  }
});

function isSameOrigin(requestUrl) {
  return requestUrl.origin === self.location.origin;
}

function shouldCacheRequest(request, requestUrl) {
  if (request.method !== 'GET' || !isSameOrigin(requestUrl)) return false;
  // Authenticated API responses can contain tenant-specific data, so keep them out of Cache Storage.
  if (requestUrl.pathname.startsWith('/api/')) return false;

  return ['document', 'script', 'style', 'image', 'font', 'manifest'].includes(request.destination);
}

async function networkFirst(request) {
  const requestUrl = new URL(request.url);

  try {
    const response = await fetch(request);

    if (response.ok && shouldCacheRequest(request, requestUrl)) {
      const cache = await caches.open(RUNTIME_CACHE);
      cache.put(request, response.clone());
    }

    return response;
  } catch (error) {
    const cachedResponse = await caches.match(request);
    if (cachedResponse) return cachedResponse;

    if (request.mode === 'navigate') {
      // Document requests get a friendly offline page instead of a browser network error.
      return caches.match('/offline.html');
    }

    throw error;
  }
}

self.addEventListener('fetch', (event) => {
  if (event.request.method !== 'GET') return;

  event.respondWith(networkFirst(event.request));
});
