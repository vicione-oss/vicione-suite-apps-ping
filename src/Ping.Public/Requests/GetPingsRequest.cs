using Sdk.Messaging;

namespace Ping.Public.Requests;

public sealed record GetPingsRequest(Guid? PingId) : IRequest<GetPingsResponse>
{
    /// <summary>
    /// if true an InvalidOperationException will be thrown on consume 
    /// </summary>
    public bool ThrowOnConsume { get; init; }
}
