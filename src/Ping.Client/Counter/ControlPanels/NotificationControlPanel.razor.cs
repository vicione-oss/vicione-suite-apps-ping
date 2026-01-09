using Microsoft.AspNetCore.Components;
using Ping.Public;
using Sdk.Authorization;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Services;

namespace Ping.Client.Counter.ControlPanels;

[ModuleAuthorize<PingClientModule>(AccessLevel.Full, Constants.CounterFeatureName)]
public sealed partial class NotificationControlPanel : ControlPanelBase<ControlPanelState>
{
    [Inject]
    private IControlPanelRegistry<PingClientModule> ControlPanelRegistry { get; init; } = default!;

    private void HideNotificationSettingsClicked()
        => ControlPanelRegistry.Remove<NotificationControlPanel>();
}
