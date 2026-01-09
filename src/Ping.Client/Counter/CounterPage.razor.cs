using Microsoft.AspNetCore.Components;
using Ping.Client.Counter.Services;
using Sdk.Client.Services;
using Sdk.MessageBanner.Contracts;

namespace Ping.Client.Counter;

public sealed partial class CounterPage
{
    private string? _description;
    private MessageType _messageType;

    [Inject] private IMessageBannerService MessageBannerService { get; set; } = default!;
    [Inject] private SimpleCounter SimpleCounter { get; set; } = default!;

    protected override string PageTitle => Localization.Common.ModuleTitle;

    private async Task CallIsolatedJavaScript()
    {
        var interop = new CounterInterop(JsInterop);
        await interop.ShowModule($"Current counter: {SimpleCounter.Value}");
    }

    private void DecrementCounter()
        => SimpleCounter.Decrement();

    private void IncrementCounter()
        => SimpleCounter.Increment();

    private void OnDescriptionChanged(ChangeEventArgs e)
    {
        if (e.Value is not null)
            _description = (string)e.Value;
    }

    private void OnMessageTypeChanged(ChangeEventArgs e)
    {
        if (e.Value is not null)
        {
            if (Enum.TryParse((string)e.Value, out MessageType type))
                _messageType = type;
        }
    }

    private void ResetCounter()
        => SimpleCounter.Reset();
}
