using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ping.Client;
using Ping.Client.Ping;
using Ping.Client.Ping.Services;
using Ping.Public.Requests;
using Sdk.Testing.Client;
using ViciOne.Ui.Blazor.Components.CheckBox.Extensions;
using ViciOne.Ui.Blazor.Components.Grid.Extensions;
using Xunit;

namespace Ping.Tests.Client.Components;

public sealed class PingPageTests
{
    [Fact]
    public void PageGetsRendered()
    {
        // Arrange
        using var ctx = new TestContext();
        ctx.SetupSuiteServices(setup =>
        {
            setup.Services.AddLocalization<PingClientModule>();

            setup.ClientMediator.Request<GetPingsRequest, GetPingsResponse>(
                Arg.Any<GetPingsRequest>(), Arg.Any<CancellationToken>())
                .Returns(new GetPingsResponse([]));
        });
        ctx.Services.AddSingleton(new PingState());
        ctx.Services.AddGridItemSelection<Guid>(typeof(PingPageServiceKey));
        ctx.Services.AddCheckBox();

        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        var rootModule = ctx.JSInterop.SetupModule("./_content/Microsoft.AspNetCore.Components.QuickGrid/QuickGrid.razor.js");
        rootModule.Mode = JSRuntimeMode.Loose;

        // Act + Assert
        var sut = ctx.RenderComponent<PingPage>();
    }
}
