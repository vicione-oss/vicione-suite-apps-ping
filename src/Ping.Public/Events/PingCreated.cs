using MassTransit;
using Sdk.Messaging;

namespace Ping.Public.Events;

[ForwardToUI]
public sealed record PingCreated(Guid PingId, DateTime Created, Guid RequestId, string Message, Guid? CorrelationId) : IEvent, CorrelatedBy<Guid?>;
