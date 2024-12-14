using System;
using System.Collections.Concurrent;
using System.Net.Http;

public class CacheManager
{
    private readonly ConcurrentDictionary<string, HttpResponseMessage> cacheStorage = new();

    public HttpResponseMessage? GetCachedResponse(string url)
    {
        return cacheStorage.TryGetValue(url, out var response) ? response : null;
    }

    public void UpdateCache(string url, HttpResponseMessage response)
    {
        cacheStorage[url] = response;
    }
}