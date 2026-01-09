using Sdk.Instance;
using Sdk.Messaging;

namespace Ping.Public.Requests;

public sealed record GetSystemInformationRequest(InstanceType RequestingInstanceType) : IInstanceDependentRequest<GetSystemInformationResponse>;
