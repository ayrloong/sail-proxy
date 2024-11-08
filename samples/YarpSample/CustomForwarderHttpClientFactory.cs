using Yarp.ReverseProxy.Forwarder;

namespace YarpSample;

public class CustomForwarderHttpClientFactory : IForwarderHttpClientFactory
{
    private readonly CustomDelegatingHandler _handler;
    private readonly ILogger<CustomForwarderHttpClientFactory> _logger;

    public CustomForwarderHttpClientFactory(CustomDelegatingHandler handler, ILogger<CustomForwarderHttpClientFactory> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
    {
        _handler.InnerHandler = new HttpClientHandler();
        var httpClient = new HttpMessageInvoker(_handler);
        return httpClient;
    }
}