using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;

namespace Ping.Client.Counter.ControlPanels.Services;

public sealed class NotificationControlPanelDescriptor(Func<string> title) : IControlPanelDescriptor<NotificationControlPanel>
{
    public Uri? IconUrl => ModuleAssetHelper.GetModuleIconUrl<PingClientModule>("cloud-connection.svg");
    public string Title => title();
}
