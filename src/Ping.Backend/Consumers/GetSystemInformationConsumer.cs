using MassTransit;
using Microsoft.Extensions.Logging;
using Ping.Public.Requests;
using Sdk.Instance;
using Sdk.Messaging;

namespace Ping.Backend.Consumers;

public sealed class GetSystemInformationConsumer(IInstanceInformationProvider provider, ILogger<GetSystemInformationConsumer> logger) :
    InstanceDependentRequestConsumer<GetSystemInformationRequest, GetSystemInformationResponse>
{
    private readonly IInstanceInformationProvider _provider = provider;
    private readonly ILogger<GetSystemInformationConsumer> _logger = logger;

    protected override Task<GetSystemInformationResponse> HandleException(ConsumeContext<GetSystemInformationRequest> context, Exception e)
    {
        _logger.LogError(e, "Consume {Request} failed.", nameof(GetSystemInformationRequest));

        return Task.FromResult(new GetSystemInformationResponse(string.Empty, new(0, e.Message)));
    }

    protected override Task<GetSystemInformationResponse> Respond(ConsumeContext<GetSystemInformationRequest> context)
        => Task.FromResult(new GetSystemInformationResponse($"Pong requested by {context.Message.RequestingInstanceType}-instance. Created on {_provider}"));
}
