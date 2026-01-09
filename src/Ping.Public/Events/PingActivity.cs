using MassTransit;
using Sdk.Messaging;

namespace Ping.Public.Events;

[ForwardToUI]
public sealed record PingActivity(Guid PingId, string Activity, Guid? CorrelationId) : IEvent, CorrelatedBy<Guid?>;
