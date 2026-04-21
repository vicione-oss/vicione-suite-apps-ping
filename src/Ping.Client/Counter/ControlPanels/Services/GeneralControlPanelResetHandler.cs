using Sdk.Client.ControlPanels.Services;

namespace Ping.Client.Counter.ControlPanels.Services;

internal sealed class GeneralControlPanelResetHandler() :
    IControlPanelResetHandler<GeneralControlPanelState>
{
    public async Task Reset(GeneralControlPanelState state, CancellationToken cancellationToken)
    {
        state.BeginLoading();
        try
        {
            state.NotificationSubjectTemplate = "Subject 123";
        }
        finally
        {
            state.EndLoading();
        }
    }
}
