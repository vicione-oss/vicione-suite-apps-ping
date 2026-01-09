using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ping.Backend.DbContext;
using Ping.Public.Events;
using Ping.Public.Requests;
using Sdk.Messaging;

namespace Ping.Backend.Consumers;

/// <summary>
/// Request consumer are handled like other consumers so they get retried on unhandled exceptions
/// If all attempts fail the message is moved to Ping_GetPings_error queue and the requester gets
/// an exception
/// </summary>
public sealed class GetPingsConsumer(IPingDbContext dbContext, ILogger<GetPingsConsumer> logger) :
    RequestConsumer<GetPingsRequest, GetPingsResponse>
{
    private readonly IPingDbContext _dbContext = dbContext;
    private readonly ILogger<GetPingsConsumer> _logger = logger;

    protected override Task<GetPingsResponse> HandleException(ConsumeContext<GetPingsRequest> context, Exception e)
    {
        if (context.Message.ThrowOnConsume && (context.Message.ThrowOnRetry || context.GetRetryAttempt() <= 0))
            throw e;

        _logger.LogError(e, "Consume {Request} failed.", nameof(GetPingsRequest));

        return Task.FromResult(new GetPingsResponse([], new ErrorInfo(PingError.UnknownError, e.Message)));
    }

    protected override async Task<GetPingsResponse> Respond(ConsumeContext<GetPingsRequest> context)
    {
        _logger.LogInformation("Consuming {Request} CorrelationId {CorrelationId}. Attempt: {Attempt}",
            nameof(GetPingsRequest), context.CorrelationId, context.GetRetryAttempt());

        if (context.Message.ThrowOnConsume && (context.Message.ThrowOnRetry || context.GetRetryAttempt() <= 0))
        {
            // if all retry attempts fail the request client throws an exception 
            throw new InvalidOperationException("This exception was requested by request");
        }

        var pingRequest = _dbContext.Pings.AsNoTracking();
        if (context.Message.PingId is not null)
            pingRequest = pingRequest.Where(p => p.Id == context.Message.PingId);

        var pingResults = await pingRequest.ToListAsync(context.CancellationToken);

        return new GetPingsResponse(pingResults);
    }
}
