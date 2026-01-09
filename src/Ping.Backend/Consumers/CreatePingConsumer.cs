using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ping.Backend.DbContext;
using Ping.Public.Commands;
using Ping.Public.Contracts;
using Ping.Public.Events;
using Ping.Public.Requests;
using Sdk.Backend.Messaging;
using Sdk.Instance;
using Sdk.Messaging;

namespace Ping.Backend.Consumers;

/// <summary>
/// Consume CreatePingCommand messages from queue Ping_CreatePingCommand using tracked activities
/// Check the consumer behavior in CreatePingReadMe.log  
/// </summary>
public sealed class CreatePingConsumer(IInstanceInformationProvider provider, ISuiteMediator mediator,
                                       IPingDbContext dbContext, ILogger<CreatePingConsumer> logger) :
    IConsumer<CreatePing>, IConsumer<Fault<CreatePing>>
{
    private readonly IInstanceInformationProvider _provider = provider;
    private readonly ISuiteMediator _mediator = mediator;
    private readonly IPingDbContext _dbContext = dbContext;
    private readonly ILogger<CreatePingConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<CreatePing> context)
    {
        _logger.LogInformation("Consuming {Command}", nameof(CreatePing));

        // this exception will put the message on the Ping_CreatePingCommand_error queue using
        // the Ping_CreatePingCommand_error exchange. it can be consumed by ConsumeContext<Fault<CreatePingCommand>>
        // the command will be retried x times
        if (context.Message.ThrowOnConsume && (context.Message.ThrowOnRetry || context.GetRetryAttempt() <= 0))
            throw new InvalidOperationException("This exception was requested by command");

        var instances = await _provider.GetInstancesInCluster(context.CancellationToken);
        var sysInfo = "No Message";
        if (instances.Any(i => i.Type != InstanceType.Master))
        {
            var targetInstance = instances.First(i => i.Type != InstanceType.Master);
            var request = new GetSystemInformationRequest(_provider.Local.Type);
            var response = await _mediator.Request<GetSystemInformationRequest, GetSystemInformationResponse>(request, targetInstance.Id, TimeSpan.FromSeconds(3), context.CancellationToken);
            sysInfo = response.SysInfo;
        }

        var pingToCreate = new PingResult
        {
            Created = DateTime.UtcNow,
            Message = sysInfo,
            Id = context.Message.PingId,
            RequestId = context.MessageId.GetValueOrDefault(),
        };

        // we simulate delay like for an expensive db operation
        if (context.Message.WantedProcessTimeMs > 0)
        {
            await Task.Delay(context.Message.WantedProcessTimeMs, context.CancellationToken);
        }

        try
        {
            var pingWithId = _dbContext.Pings.Add(pingToCreate).Entity;
            if (await _dbContext.Instance.SaveChangesAsync(context.CancellationToken) > 0)
            {
                await context.Publish(new PingCreated(pingWithId.Id, pingWithId.Created, pingWithId.RequestId, pingWithId.Message ?? "No Message", context.CorrelationId));
            }
        }
        catch (DbUpdateException e)
        {
            _logger.LogWarning(e, "Create failed on ping {PingId}. Retry: {Times}", context.Message.PingId, context.GetRetryCount());
            throw;
        }
    }

    /// <summary>
    /// called when an exception happened on consume and all retry attempts failed
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<Fault<CreatePing>> context)
    {
        _logger.LogError("{Command} faulted with errors: {Errors}",
            nameof(CreatePing),
            string.Join(" | ", context.Message.Exceptions.Select(e => e.Message)));

        await context.Publish(
            new PingError(
                new ErrorInfo(PingError.CreateFailed, context.Message.Exceptions.Last().Message),
                context.Message.Message.PingId, context.CorrelationId));
    }
}
