using Yarp.ReverseProxy.Forwarder;

namespace Yarp.Extensions.Resilience;

public class ResilienceForwarderHttpClientFactory(DelegatingHandler delegatingHandler) : IForwarderHttpClientFactory
{
    public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
    {
        delegatingHandler.InnerHandler = new HttpClientHandler();
        var httpClient = new HttpMessageInvoker(delegatingHandler);
        return httpClient;
    }
}