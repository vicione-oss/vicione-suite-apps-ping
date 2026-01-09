using Sdk.Messaging;

namespace Ping.Public.Commands;

public sealed record RemovePing(Guid PingId) : ICommand
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// if true an InvalidOperationException will be thrown on activity execution 
    /// </summary>
    public bool ThrowOnActivity { get; init; }

    /// <summary>
    /// if true an InvalidOperationException will be thrown on consume 
    /// </summary>
    public bool ThrowOnConsume { get; init; }

    /// <summary>
    /// if true an InvalidOperationException will be thrown on consume/activity on retry attempts 
    /// </summary>
    public bool ThrowOnRetry { get; init; }
}
