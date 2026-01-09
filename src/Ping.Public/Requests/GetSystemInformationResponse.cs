using Sdk.Messaging;

namespace Ping.Public.Requests;

public sealed record GetSystemInformationResponse(string SysInfo, ErrorInfo? RequestError = null) : IResponse;
