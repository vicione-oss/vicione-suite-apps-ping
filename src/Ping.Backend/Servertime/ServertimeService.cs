using Microsoft.Extensions.Hosting;
using Sdk.Backend.Messaging;

namespace Ping.Backend.Servertime;

public sealed class ServertimeService(ISuiteMediator mediator) : BackgroundService
{
    private readonly ISuiteMediator _mediator = mediator;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _mediator.Publish(new Public.Events.Servertime(), stoppingToken);
            await Task.Delay(60_000, stoppingToken);
        }
    }
}
