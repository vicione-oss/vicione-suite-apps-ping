using Ping.Client.Ping.Localization;
using Sdk.Client.Modules.Localization;

namespace Ping.Client;

internal sealed class PingClientLocalizer : IClientModuleLocalizer<PingClientModule>
{
    public string? GetDescription() => Common.ModuleDescription;
    public string GetTitle() => Common.ModuleTitle;
}
