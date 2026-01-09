using Ping.Public.Contracts;
using Sdk.Messaging;

namespace Ping.Public.Requests;

public sealed record GetPingsResponse(List<PingResult> Result, ErrorInfo? RequestError = null) : IResponse;
