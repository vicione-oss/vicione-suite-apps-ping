using Microsoft.AspNetCore.Components;
using Ping.Client.Counter.Services;
using Sdk.Client.NotificationArea.Components;

namespace Ping.Client.Counter.NotificationArea;

internal sealed class CounterNotificationElementBadge : NotificationElementNumberBadgeBase, IDisposable
{
    [Inject] public required SimpleCounter SimpleCounter { get; set; }

    public void Dispose()
    {
        SimpleCounter.Changed -= OnSimpleCounterChanged;

        GC.SuppressFinalize(this);
    }

    protected override int? GetNumber()
        => SimpleCounter.Value != 0 ? SimpleCounter.Value : null;

    protected override void OnInitialized()
        => SimpleCounter.Changed += OnSimpleCounterChanged;

    private void OnSimpleCounterChanged()
        => InvokeAsync(StateHasChanged);
}
