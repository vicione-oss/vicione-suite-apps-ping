using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Components;
using ViciOne.Ui.MonochromeIcons.Core.Enums;
using ViciOne.Ui.MonochromeIcons.Core.Extensions;

namespace Ping.Client.Counter.NotificationArea;

public sealed partial class CounterNotificationElementFlyoutContent : ComponentBase, INotificationElementFlyoutContent
{
    private readonly string _externalLinkIconCssClasses = MonochromeIconName.ExternalLink.GetCssClasses(MonochromeIconSize.Small).ToSpaceSeparated();

    [Inject] public required NavigationManager NavigationManager { get; set; }

    private void NavigateToCounterPage()
        => NavigationManager.NavigateTo(PingClientModule.CounterPageRoute, false);
}
