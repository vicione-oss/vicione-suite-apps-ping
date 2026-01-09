using Microsoft.JSInterop;
using Sdk.Client.Services;

namespace Ping.Client.Counter;

internal sealed class CounterInterop(IJsInterop jsInterop)
{
    private readonly IJsInterop _jsInterop = jsInterop;

    internal async Task ShowModule(string? anchor)
    {
        var jsModuleReference = await _jsInterop.IncludeModuleScript<PingClientModule>("counter-interop.js");

        if (jsModuleReference is not null)
        {
            var counter = await jsModuleReference.InvokeAsync<IJSObjectReference>("init");

            await counter.InvokeVoidAsync("showModule", anchor);
        }
    }
}
