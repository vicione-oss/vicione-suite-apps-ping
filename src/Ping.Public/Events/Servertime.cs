using Sdk.Messaging;

namespace Ping.Public.Events;

[ForwardToUI]
public sealed record Servertime : IEvent
{
    public DateTime Time { get; init; } = DateTime.UtcNow;
}
