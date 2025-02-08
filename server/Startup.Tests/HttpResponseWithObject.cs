namespace Startup.Tests;

public class HttpResponseWithObject<T>
{
    public HttpResponseMessage HttpResponseMessage { get; set; }
    public T Object { get; set; }
}