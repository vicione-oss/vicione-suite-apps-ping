using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ping.Backend.DbContext;
using Ping.Public.Commands;
using Ping.Public.Events;
using Sdk.Messaging;

namespace Ping.Backend.Consumers;

/// <summary>
/// Consume RemovePingCommand from Ping_RemovePingCommand queue using untracked execute activity
/// Check the consumer behavior in RemovePingReadMe.log  
/// </summary>
public sealed class RemovePingConsumer(IPingDbContext dbContext, ILogger<RemovePingConsumer> logger) :
    IConsumer<RemovePing>, IConsumer<Fault<RemovePing>>
{
    private readonly IPingDbContext _dbContext = dbContext;
    private readonly ILogger<RemovePingConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<RemovePing> context)
    {
        _logger.LogInformation("Consume {Command} PingId:{PingId} Attempt:{Attempt}",
            nameof(RemovePing),
            context.Message.PingId,
            context.GetRetryAttempt());

        // this exception will put the message on the Ping_RemovePingCommand_error queue using
        // the Ping_RemovePingCommand_error exchange. it can be consumed by ConsumeContext<Fault<RemovePingCommand>>
        if (context.Message.ThrowOnConsume && (context.Message.ThrowOnRetry || context.GetRetryAttempt() <= 0))
        {
            throw new InvalidOperationException("This exception was requested by command");
        }

        var ping = await _dbContext.Pings.SingleOrDefaultAsync(p => p.Id == context.Message.PingId,
            context.CancellationToken);
        if (ping is null)
            return;

        try
        {
            var deletedPing = _dbContext.Pings.Remove(ping).Entity;
            if (await _dbContext.Instance.SaveChangesAsync(context.CancellationToken) > 0)
            {
                // this event just transports the id and the ui can act directly on it
                // therefore it doesn't matter if the event is triggered by master before
                // the change is applied on the local slave database
                await context
                    .Publish(new PingDeleted(deletedPing.Id, context.CorrelationId))
                    .ConfigureAwait(false);
            }
        }
        catch (DbUpdateException e)
        {
            _logger.LogWarning(e, "Delete failed on ping {PingId}. Retry: {Times}", context.Message.PingId, context.GetRetryCount());

            throw;
        }
    }

    /// <summary>
    /// only called if an exception happened directly in the consume call and all retry attempts have failed
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<Fault<RemovePing>> context)
    {
        _logger.LogError("{Command} faulted with errors: {Errors}",
            nameof(CreatePing),
            string.Join(" | ", context.Message.Exceptions.Select(e => e.Message)));

        await context.Publish(
            new PingError(
                new ErrorInfo(PingError.DeleteFailed, context.Message.Exceptions.Last().Message),
                context.Message.Message.PingId, context.CorrelationId));
    }
}
