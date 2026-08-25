using Microsoft.AspNetCore.Components;
using Ping.Client.Counter.Services;
using Sdk.Client.Services;
using Sdk.MessageBanner.Contracts;
using ViciOne.Ui.Blazor.Components.ComboBox;

namespace Ping.Client.Counter;

public sealed partial class CounterPage
{
    private readonly IReadOnlyList<ComboBoxItem<MessageType, string>> _messageTypes =
        [.. Enum.GetValues<MessageType>().Select(type => new ComboBoxItem<MessageType, string> { Value = type, Text = type.ToString() })];

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

    private void OnDescriptionChanged(string? description)
        => _description = description;

    private void OnMessageTypeChanged(MessageType messageType)
        => _messageType = messageType;

    private void ResetCounter()
        => SimpleCounter.Reset();
}
