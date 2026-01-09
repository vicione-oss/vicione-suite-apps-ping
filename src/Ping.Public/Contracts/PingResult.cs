namespace Ping.Public.Contracts;

public sealed class PingResult
{
    public DateTime Created { get; init; }
    public Guid Id { get; init; }
    public string? Message { get; set; }
    public Guid RequestId { get; init; }
}
