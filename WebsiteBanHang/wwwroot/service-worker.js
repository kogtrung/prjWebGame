// Service Worker vô hiệu hóa - không cache
self.addEventListener('install', function(event) {
  self.skipWaiting();
});

self.addEventListener('activate', function(event) {
  // Xóa tất cả cache cũ
  event.waitUntil(
    caches.keys().then(function(cacheNames) {
      return Promise.all(
        cacheNames.map(function(cacheName) {
          return caches.delete(cacheName);
        })
      );
    }).then(function() {
      return self.clients.claim();
    })
  );
});

// Không cache bất kỳ yêu cầu nào, luôn lấy từ network
self.addEventListener('fetch', function(event) {
  event.respondWith(fetch(event.request));
}); 