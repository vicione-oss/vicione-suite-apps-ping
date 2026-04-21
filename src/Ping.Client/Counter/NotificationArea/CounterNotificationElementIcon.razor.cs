using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Services;
using ViciOne.Ui.MonochromeIcons.Core.Enums;

namespace Ping.Client.Counter.NotificationArea;

public sealed partial class CounterNotificationElementIcon : ComponentBase, IDisposable
{
    [CascadingParameter]
    public required NotificationElementState NotificationElementState { get; set; }

    public void Dispose()
        => NotificationElementState.Changed -= OnNotificationElementStateChanged;

    private MonochromeIconName GetName()
        => NotificationElementState.IsActive ? MonochromeIconName.Timer : MonochromeIconName.TimeOutLight;

    protected override void OnInitialized()
        => NotificationElementState.Changed += OnNotificationElementStateChanged;

    private void OnNotificationElementStateChanged(NotificationElementStateChangedEventArgs args)
    {
        if (args.PropertyNames.Contains(nameof(NotificationElementState.IsActive)))
            InvokeAsync(StateHasChanged);
    }
}
