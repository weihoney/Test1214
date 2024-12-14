using System.Net.Http;
using System.Threading.Tasks;

public class NetworkRequest
{
    public string Url { get; }
    public HttpMethod Method { get; }
    public HttpContent? Content { get; }
    public int Priority { get; }
    public TaskCompletionSource<HttpResponseMessage> TaskCompletionSource { get; } = new();

    public NetworkRequest(string url, HttpMethod method, int priority, HttpContent? content = null)
    {
        Url = url;
        Method = method;
        Priority = priority;
        Content = content;
    }

    public HttpRequestMessage ToHttpRequestMessage()
    {
        var requestMessage = new HttpRequestMessage(Method, Url)
        {
            Content = Content
        };
        return requestMessage;
    }
}