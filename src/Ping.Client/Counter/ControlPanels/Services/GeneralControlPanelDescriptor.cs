using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;
using ViciOne.Ui.Localization.Resources;

namespace Ping.Client.Counter.ControlPanels.Services;

public sealed class GeneralControlPanelDescriptor : IControlPanelDescriptor<GeneralControlPanel>
{
    public Uri? IconUrl => ModuleAssetHelper.GetModuleIconUrl<PingClientModule>("fan.svg");
    public string Title => CommonVocabulary.General;
}
