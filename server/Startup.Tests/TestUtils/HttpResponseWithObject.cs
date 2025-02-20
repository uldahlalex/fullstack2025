namespace Startup.Tests.TestUtils;

public class HttpResponseWithObject<T>
{
    public HttpResponseMessage HttpResponseMessage { get; set; }
    public T Object { get; set; }
}