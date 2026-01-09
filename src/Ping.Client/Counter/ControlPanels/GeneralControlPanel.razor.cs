using System.Timers;
using Microsoft.AspNetCore.Components;
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
    private bool _hasReloadingTab;
    private readonly string _onOffIconCssClasses = MonochromeIconName.OnOff.GetCssClasses(MonochromeIconSize.Small).ToSpaceSeparated();

    private sealed class NotificationMetadata
    {
        public required string Placeholder { get; init; }
        public string? Value { get; set; }
    }

    private readonly List<NotificationMetadata> _notificationMetadata = [new NotificationMetadata { Placeholder = "Meta data 1" }];
    private string? _notificationSubjectTemplate = "Subject 123";

    private class ComboBoxItem<TValue, TText>
    {
        public required TText Text { get; init; }
        public required TValue Value { get; init; }
    }

    private readonly List<ComboBoxItem<bool?, string>> _editSettingsGroupComboBoxItems = [.. new[]
    {
        new ComboBoxItem<bool?, string> { Value = null, Text = "Null value, show by default" },
        new ComboBoxItem<bool?, string> { Value = true, Text = "Show" },
        new ComboBoxItem<bool?, string> { Value = false, Text = "Hide" }
    }.OrderBy(i => i.Text)];

    private bool? _editSettingsGroupExpanded;
    private bool _notificationsSettingsGroupExpanded;
    private bool _notificationsHiddenUiElementsSettingsGroupExpanded;
    private bool _reloadingPopupSettingsGroupExpanded;
    private SettingsInformationKind _settingsInformationKind;
    private bool _moreInformationSettingsGroupExpanded;
    private bool _addendumSettingsGroupExpanded = true;

    [Inject]
    private IControlPanelRegistry<PingClientModule> ControlPanelRegistry { get; init; } = default!;

    [Inject]
    private ILayoutService LayoutService { get; init; } = default!;

    private void AddNotificationMetadata()
        => _notificationMetadata.Add(new NotificationMetadata { Placeholder = $"Meta data {_notificationMetadata.Count + 1}" });

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
        _hasReloadingTab = !_hasReloadingTab;

        if (_hasReloadingTab)
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
        => _notificationMetadata.RemoveAt(_notificationMetadata.Count - 1);
}
