using Ping.Client.Counter.Localization;
using Sdk.Client.NotificationArea.Attributes;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.NotificationArea.Services;

namespace Ping.Client.Counter.NotificationArea;

[InitialNotificationElement<PingClientModule>]
public sealed class CounterNotificationElement : NotificationElement<NotificationElementState, CounterNotificationElementIcon>
{
    public CounterNotificationElement()
    {
        RegisterBadge<CounterNotificationElementBadge>();
        RegisterFlyout<CounterNotificationElementFlyout>();
    }

    protected override string GetTitle()
        => Common.ModuleTitle;
}
