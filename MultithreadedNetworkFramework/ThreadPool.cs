using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class ThreadPool
{
    private readonly int maxThreads;
    private int activeThreads = 0;
    private readonly ConcurrentQueue<Func<Task>> taskQueue = new();

    public ThreadPool(int maxThreads)
    {
        this.maxThreads = maxThreads;
    }

    public bool HasAvailableThread()
    {
        return activeThreads < maxThreads;
    }

    public void ExecuteTask(Func<Task> task)
    {
        if (activeThreads < maxThreads)
        {
            Interlocked.Increment(ref activeThreads);
            RunTask(task);
        }
        else
        {
            taskQueue.Enqueue(task);
        }
    }

    private async void RunTask(Func<Task> task)
    {
        try
        {
            await task();
        }
        finally
        {
            Interlocked.Decrement(ref activeThreads);

            if (taskQueue.TryDequeue(out var nextTask))
            {
                RunTask(nextTask);
            }
        }
    }
}