namespace Signer.Services;

public class LoggingHttpMessageHandler : DelegatingHandler
{
    public LoggingHttpMessageHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Request: {request.Method} {request.RequestUri}");

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine($"Response: {(int)response.StatusCode} {response.StatusCode}");

        return response;
    }
}