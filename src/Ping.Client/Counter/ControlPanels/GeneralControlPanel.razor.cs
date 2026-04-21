using System.Timers;
using Microsoft.AspNetCore.Components;
using Ping.Client.Counter.ControlPanels.Services;
using Sdk.Client.ControlPanels.Attributes;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Services;
using ViciOne.Ui.Localization.Resources;
using ViciOne.Ui.MonochromeIcons.Core.Enums;
using ViciOne.Ui.MonochromeIcons.Core.Extensions;

namespace Ping.Client.Counter.ControlPanels;

[ControlPanelCategory<ControlPanelCategoryDescriptor>]
public sealed partial class GeneralControlPanel
{
    private readonly System.Timers.Timer _reloadControlPanelTimer = new(3000) { AutoReset = false };
    private readonly System.Timers.Timer _reloadPopupTimer = new(3000) { AutoReset = false };
    private readonly string _onOffIconCssClasses = MonochromeIconName.OnOff.GetCssClasses(MonochromeIconSize.Small).ToSpaceSeparated();    

    [Inject]
    private IControlPanelRegistry<PingClientModule> ControlPanelRegistry { get; init; } = default!;

    [Inject]
    private ILayoutService LayoutService { get; init; } = default!;

    private void AddNotificationMetadata() => State.AddNotificationMetadata();

    protected override ValueTask DisposeAsyncCore()
    {
        if (_reloadControlPanelTimer.Enabled)
        {
            _reloadControlPanelTimer.Stop();

            State.EndLoading();
        }

        _reloadControlPanelTimer.Dispose();

        _reloadPopupTimer.Dispose();
        if (_reloadPopupTimer.Enabled)
        {
            _reloadPopupTimer.Stop();

            LayoutService.IsLoadingOverlayVisible = false;
        }

        _reloadPopupTimer.Dispose();

        return base.DisposeAsyncCore();
    }

    private void OnReloadControlPanel()
    {
        if (!_reloadControlPanelTimer.Enabled)
        {
            _reloadControlPanelTimer.Elapsed -= ReloadControlPanelTimerElapsed;
            _reloadControlPanelTimer.Elapsed += ReloadControlPanelTimerElapsed;

            State.BeginLoading();

            _reloadControlPanelTimer.Start();
        }
    }

    private void OnReloadPopup()
    {
        if (!_reloadPopupTimer.Enabled)
        {
            _reloadPopupTimer.Elapsed -= ReloadPopupTimerElapsed;
            _reloadPopupTimer.Elapsed += ReloadPopupTimerElapsed;

            LayoutService.IsLoadingOverlayVisible = true;

            _reloadPopupTimer.Start();
        }
    }

    private void OnRevertDirtyState()
        => CancelEdit();

    private void OnSetDirtyState()
        => BeginEdit();

    private void OnToggleReloadingTab()
    {
        State.HasReloadingTab = !State.HasReloadingTab;

        if (State.HasReloadingTab)
            State.ActivePageIndex = 2;
        else
            StateHasChanged();
    }

    private void OnUnlockNotificationSettings()
    {
        if (ControlPanelRegistry.All(i => i.ComponentType != typeof(NotificationControlPanel)))
        {
            ControlPanelRegistry.BeginUpdate();
            try
            {
                ControlPanelRegistry.Add<NotificationControlPanel, ControlPanelState>(
                    new NotificationControlPanelDescriptor(title: () => CommonVocabulary.NotificationPlural + " 1"),
                    new ControlPanelState(),
                    new ControlPanelCategoryDescriptor());

                ControlPanelRegistry.Add<NotificationControlPanel, ControlPanelState>(
                    new NotificationControlPanelDescriptor(title: () => CommonVocabulary.NotificationPlural + " 2"),
                    new ControlPanelState(),
                    new ControlPanelCategoryDescriptor());
            }
            finally
            {
                ControlPanelRegistry.EndUpdate();
            }
        }
    }

    private void ReloadControlPanelTimerElapsed(object? sender, ElapsedEventArgs e)
        => State.EndLoading();

    private void ReloadPopupTimerElapsed(object? sender, ElapsedEventArgs e)
        => LayoutService.IsLoadingOverlayVisible = false;

    private void RemoveNotificationMetadata()
        => State.Metadata.RemoveAt(State.Metadata.Count - 1);
}
