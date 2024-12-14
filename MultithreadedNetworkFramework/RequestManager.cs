using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

public class RequestManager
{
    private readonly PriorityQueue<NetworkRequest, int> requestQueue = new();
    private readonly ThreadPool threadPool;
    private readonly CacheManager cacheManager = new();
    private readonly MiddlewareManager middlewareManager = new();

    public RequestManager(int maxThreads)
    {
        threadPool = new ThreadPool(maxThreads);
    }

    public async Task<HttpResponseMessage> AddRequest(NetworkRequest request)
    {
        var cachedResponse = cacheManager.GetCachedResponse(request.Url);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }

        await middlewareManager.ExecuteMiddlewareAsync("BeforeRequest", request);

        lock (requestQueue)
        {
            requestQueue.Enqueue(request, request.Priority);
        }

        DispatchRequests();

        return await request.TaskCompletionSource.Task;
    }

    private void DispatchRequests()
    {
        while (requestQueue.Count > 0 && threadPool.HasAvailableThread())
        {
            lock (requestQueue)
            {
                if (requestQueue.TryDequeue(out var request, out _))
                {
                    threadPool.ExecuteTask(() => HandleRequestAsync(request));
                }
            }
        }
    }

    private async Task HandleRequestAsync(NetworkRequest request)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request.ToHttpRequestMessage());

            cacheManager.UpdateCache(request.Url, response);

            await middlewareManager.ExecuteMiddlewareAsync("AfterResponse", response);

            request.TaskCompletionSource.SetResult(response);
        }
        catch (Exception ex)
        {
            if (RetryPolicy.ShouldRetry(request, ex))
            {
                await Task.Delay(RetryPolicy.GetRetryDelay(request));
                await AddRequest(request);
            }
            else
            {
                request.TaskCompletionSource.SetException(ex);
            }
        }
    }
}
