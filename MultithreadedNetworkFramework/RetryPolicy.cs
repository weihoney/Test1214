using System;
using System.Threading.Tasks;

public static class RetryPolicy
{
    private const int MaxRetries = 3;
    private const int BaseDelayMilliseconds = 500;

    public static bool ShouldRetry(NetworkRequest request, Exception exception)
    {
        return request.TaskCompletionSource.Task.Status != TaskStatus.RanToCompletion &&
               request.Priority > 0;
    }

    public static int GetRetryDelay(NetworkRequest request)
    {
        return BaseDelayMilliseconds * (MaxRetries - request.Priority);
    }
}