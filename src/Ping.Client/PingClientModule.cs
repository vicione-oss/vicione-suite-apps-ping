using Microsoft.Extensions.DependencyInjection;
using Ping.Client.Counter.ControlPanels;
using Ping.Client.Counter.ControlPanels.Services;
using Ping.Client.Counter.Services;
using Ping.Client.Ping;
using Ping.Client.Ping.Services;
using Ping.Public;
using Sdk.Authorization.Extensions;
using Sdk.Client.ControlPanels.Extensions;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;
using Sdk.Client.Modules.Localization.Extensions;
using Sdk.Client.NavTiles.Extensions;
using Sdk.Client.NotificationArea.Extensions;
using ViciOne.Ui.Blazor.Components.CheckBox.Extensions;
using ViciOne.Ui.Blazor.Components.Grid.Extensions;
using ViciOne.Ui.Blazor.Components.SpinEdit.Extensions;

namespace Ping.Client;

/// <summary>
/// the module resource translations is needed by the client
/// </summary>
public sealed class PingClientModule : ClientModule
{
    public const string PingPageRoute = "/ping";
    public const string BuilderPageRoute = "/builder";
    public const string CounterPageRoute = "/counter";

    public override Action<IServiceCollection> Configure => (services) =>
    {
        services.AddLocalization<PingClientModule, PingClientLocalizer>();
        services.AddNavTiles<PingClientModule>();
        services.AddNotificationElements<PingClientModule>();

        services.AddControlPanel<PingClientModule, GeneralControlPanel, GeneralControlPanelState>()
            .WithAutoDiscovery<GeneralControlPanelDescriptor>()
            .WithResetHandler<GeneralControlPanelResetHandler>();

        services.AddControlPanel<PingClientModule, NotificationControlPanel, ControlPanelState>();

        // ping
        services.AddCheckBox();
        services.AddIntSpinEdit();

        services.AddScoped<PingState>();
        services.AddGridItemSelection<Guid>(typeof(PingPageServiceKey));

        services.AddModuleFeature<PingClientModule>(Constants.CounterFeatureName, "Controls access to the counter feature");
        services.AddModuleFeature<PingClientModule>(Constants.ProgressFeatureName, "Controls access to the progress NavTile");

        // counter
        services.AddScoped<SimpleCounter>();
    };
}
