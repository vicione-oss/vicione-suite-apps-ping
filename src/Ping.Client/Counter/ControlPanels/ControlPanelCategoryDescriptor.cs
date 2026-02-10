using Ping.Client.Counter.Localization;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;

namespace Ping.Client.Counter.ControlPanels;

public sealed class ControlPanelCategoryDescriptor : IControlPanelCategoryDescriptor
{
    public string? IconCssClass => null;
    public Uri? IconUrl => ModuleAssetHelper.GetModuleIconUrl<PingClientModule>("fan.svg");
    public int? Position => 0;
    public string Title => Common.ModuleTitle;
}
