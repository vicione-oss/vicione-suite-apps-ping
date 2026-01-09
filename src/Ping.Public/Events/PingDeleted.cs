using MassTransit;
using Sdk.Messaging;

namespace Ping.Public.Events;

[ForwardToUI]
public sealed record PingDeleted(Guid PingId, Guid? CorrelationId) : IEvent, CorrelatedBy<Guid?>;
