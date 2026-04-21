using Microsoft.Extensions.Logging;
using Ping.Public.Requests;
using Sdk.Backend.Messaging;
using Sdk.Instance;

namespace Ping.Backend.Consumers;

public sealed class GetSystemInformationConsumer(IInstanceInformationProvider provider, ILogger<GetSystemInformationConsumer> logger) :
    InstanceDependentRequestConsumer<GetSystemInformationRequest, GetSystemInformationResponse>
{
    private readonly IInstanceInformationProvider _provider = provider;
    private readonly ILogger<GetSystemInformationConsumer> _logger = logger;

    public override Task<GetSystemInformationResponse> HandleException(GetSystemInformationRequest message, Exception e, CancellationToken cancellationToken)
    {
        _logger.LogError(e, "Consume {Request} failed.", nameof(GetSystemInformationRequest));

        return Task.FromResult(new GetSystemInformationResponse(string.Empty, new(0, e.Message)));
    }

    public override Task<GetSystemInformationResponse> Respond(GetSystemInformationRequest message, CancellationToken cancellationToken)
        => Task.FromResult(new GetSystemInformationResponse($"Pong requested by {message.RequestingInstanceType}-instance. Created on {_provider}"));
}
