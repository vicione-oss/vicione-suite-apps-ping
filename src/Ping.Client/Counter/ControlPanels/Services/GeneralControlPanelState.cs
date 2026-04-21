using Sdk.Client.ControlPanels.Services;
using ViciOne.Ui.Blazor.Components.ComboBox;

namespace Ping.Client.Counter.ControlPanels.Services;

public sealed class GeneralControlPanelState : ControlPanelState
{

    internal sealed class NotificationMetadata
    {
        public required string Placeholder { get; init; }
        public string? Value { get; set; }
    }

    internal List<NotificationMetadata> Metadata { get; set; } = [new NotificationMetadata { Placeholder = "Meta data 1" }];

    public bool HasReloadingTab { get; set; }
    public string? NotificationSubjectTemplate { get; set; } = "Subject 123";
    public bool? EditSettingsGroupExpanded { get; set; }
    public bool NotificationsSettingsGroupExpanded { get; set; }
    public bool NotificationsHiddenUiElementsSettingsGroupExpanded { get; set; }
    public bool ReloadingPopupSettingsGroupExpanded { get; set; }
    internal SettingsInformationKind SettingsInformationKind { get; set; }
    public bool MoreInformationSettingsGroupExpanded { get; set; }
    public bool AddSettingsGroupExpanded { get; set; } = true;

    public List<ComboBoxItem<bool?, string>> EditSettingsGroupComboBoxItems { get; } = [.. new[]
    {
        new ComboBoxItem<bool?, string> { Value = null, Text = "Null value, show by default" },
        new ComboBoxItem<bool?, string> { Value = true, Text = "Show" },
        new ComboBoxItem<bool?, string> { Value = false, Text = "Hide" }
    }.OrderBy(i => i.Text)];

    internal void AddNotificationMetadata()
        => Metadata.Add(new NotificationMetadata { Placeholder = $"Meta data {Metadata.Count + 1}" });
}
