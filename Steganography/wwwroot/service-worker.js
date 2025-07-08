self.addEventListener('install', function (e) {
    console.log('Service Worker: Installed');
});

self.addEventListener('fetch', function (event) {
    // Can add caching logic here
});