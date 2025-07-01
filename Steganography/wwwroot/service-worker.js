﻿const CACHE_NAME = "stego-cache-v1";
const OFFLINE_URL = "/offline.html";

self.addEventListener("install", (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => {
            return cache.addAll(["/", "/manifest.json", "/css/site.css", OFFLINE_URL]);
        })
    );
    self.skipWaiting();
});

self.addEventListener("fetch", (event) => {
    event.respondWith(
        fetch(event.request).catch(() => caches.match(event.request).then(
            (response) => response || caches.match(OFFLINE_URL)
        ))
    );
});