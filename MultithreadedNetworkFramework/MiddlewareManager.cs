using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MiddlewareManager
{
    private readonly Dictionary<string, List<Func<object, Task>>> middlewares = new();

    public MiddlewareManager()
    {
        middlewares["BeforeRequest"] = new List<Func<object, Task>>();
        middlewares["AfterResponse"] = new List<Func<object, Task>>();
    }

    public void RegisterMiddleware(string type, Func<object, Task> middleware)
    {
        if (middlewares.ContainsKey(type))
        {
            middlewares[type].Add(middleware);
        }
    }

    public async Task ExecuteMiddlewareAsync(string type, object context)
    {
        if (middlewares.ContainsKey(type))
        {
            foreach (var middleware in middlewares[type])
            {
                await middleware(context);
            }
        }
    }
}