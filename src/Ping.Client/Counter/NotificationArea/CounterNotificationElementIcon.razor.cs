using Microsoft.AspNetCore.Components;
using Sdk.Client.Enums;
using Sdk.Client.NotificationArea.Services;

namespace Ping.Client.Counter.NotificationArea;

public sealed partial class CounterNotificationElementIcon : ComponentBase, IDisposable
{
    [CascadingParameter]
    public required NotificationElementState NotificationElementState { get; set; }

    public void Dispose()
        => NotificationElementState.Changed -= OnNotificationElementStateChanged;

    private MaterialDesignIconName GetName()
        => NotificationElementState.IsActive ? MaterialDesignIconName.ProgressClock : MaterialDesignIconName.ClockOutline;

    protected override void OnInitialized()
        => NotificationElementState.Changed += OnNotificationElementStateChanged;

    private void OnNotificationElementStateChanged(NotificationElementStateChangedEventArgs args)
    {
        if (args.PropertyNames.Contains(nameof(NotificationElementState.IsActive)))
            InvokeAsync(StateHasChanged);
    }
}
