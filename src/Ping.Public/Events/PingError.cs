using MassTransit;
using Sdk.Messaging;

namespace Ping.Public.Events;

[ForwardToUI]
public sealed record PingError(ErrorInfo Error, Guid? PingId, Guid? CorrelationId) : IEvent, CorrelatedBy<Guid?>
{
    public const int UnknownError = 0;
    public const int CreateFailed = 100;
    public const int DeleteFailed = 200;
    public const int DeleteFailedNotFound = 201;
}
