using Ping.Client.Counter.Localization;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.NotificationArea.Services;

namespace Ping.Client.Counter.NotificationArea;

internal sealed class CounterNotificationElementFlyout : NotificationElementFlyout<NotificationElementState, CounterNotificationElementFlyoutContent>
{
    protected override string? GetHeading() => Common.ModuleTitle;
}
