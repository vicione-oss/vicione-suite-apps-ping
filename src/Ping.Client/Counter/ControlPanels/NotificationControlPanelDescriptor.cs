using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;

namespace Ping.Client.Counter.ControlPanels;

public sealed class NotificationControlPanelDescriptor(Func<string> title) : IControlPanelDescriptor<NotificationControlPanel>
{
    public string IconPath => ModuleAssetHelper.GetModuleIconPath<PingClientModule>("cloud-connection.svg");
    public string Title => title();
}
