using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Services;

namespace Ping.Client.Counter.ControlPanels;

public class CounterControlPanelBase<TState> : ControlPanelBase<TState>
    where TState : IControlPanelState
{
}
