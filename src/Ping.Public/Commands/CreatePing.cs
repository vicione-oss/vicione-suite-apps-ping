using Sdk.Messaging;

namespace Ping.Public.Commands;

public sealed record CreatePing(Guid PingId, int WantedProcessTimeMs) : ICommand
{
    /// <summary>
    /// events related to the command can be correlated using this id
    /// </summary>
    public Guid CorrelationId => PingId;

    /// <summary>
    /// if true an InvalidOperationException will be thrown on consume 
    /// </summary>
    public bool ThrowOnConsume { get; init; }

    /// <summary>
    /// if true an InvalidOperationException will be thrown on consume/activity on retry attempts 
    /// </summary>
    public bool ThrowOnRetry { get; init; }
}
