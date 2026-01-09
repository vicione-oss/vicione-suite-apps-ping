using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Components;

namespace Ping.Client.Counter.NotificationArea;

public sealed partial class CounterNotificationElementFlyoutContent : ComponentBase, INotificationElementFlyoutContent
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
}
