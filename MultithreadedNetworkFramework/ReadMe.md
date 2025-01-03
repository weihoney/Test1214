﻿核心模块设计
1. RequestManager (请求管理器)
   负责接收、调度和管理所有的网络请求。


    维护请求队列。
    控制请求优先级。
    分发任务到线程池。
2. ThreadPool (线程池)
   用于管理多个工作线程，每个线程独立处理一个或多个网络请求。

    
    限制并发线程数。
    动态调整线程池大小。
    回收空闲线程。
3. CacheManager (缓存管理器)
   提供基于 URL 和数据的缓存策略（如强缓存、弱缓存）。

    
    根据配置决定是否从缓存返回响应。
    更新缓存内容。
4. MiddlewareManager (中间件管理器)
   允许开发者插入中间件，拦截请求和响应，进行修改或扩展。

    
    提供接口用于注册中间件。
    执行中间件链。
5. RetryPolicy (重试策略)
   提供对网络请求失败的重试机制。

    
    根据错误类型和重试规则（如指数退避）重新尝试请求。
    设置最大重试次数和时间。
