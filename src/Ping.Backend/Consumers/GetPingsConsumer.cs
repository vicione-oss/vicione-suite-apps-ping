using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ping.Backend.DbContext;
using Ping.Public.Events;
using Ping.Public.Requests;
using Sdk.Backend.Messaging;
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

    public override Task<GetPingsResponse> HandleException(GetPingsRequest message, Exception e, CancellationToken cancellationToken)
    {
        if (message.ThrowOnConsume)
            throw e;

        _logger.LogError(e, "Consume {Request} failed.", nameof(GetPingsRequest));

        return Task.FromResult(new GetPingsResponse([], new ErrorInfo(PingError.UnknownError, e.Message)));
    }

    public override async Task<GetPingsResponse> Respond(GetPingsRequest message, CancellationToken cancellationToken)
    {
        if (message.ThrowOnConsume)
        {
            // if all retry attempts fail the request client throws an exception 
            throw new InvalidOperationException("This exception was requested by request");
        }

        var pingRequest = _dbContext.Pings.AsNoTracking();
        if (message.PingId is not null)
            pingRequest = pingRequest.Where(p => p.Id == message.PingId);

        var pingResults = await pingRequest.ToListAsync(cancellationToken);

        return new GetPingsResponse(pingResults);
    }
}
