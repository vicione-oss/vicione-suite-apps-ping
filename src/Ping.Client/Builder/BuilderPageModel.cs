using System.ComponentModel.DataAnnotations;
using ViciOne.Ui.Blazor.Components.ComboBox;

namespace Ping.Client.Builder;

internal sealed class BuilderPageModel
{
    [Required]
    public string? Component { get; set; }

    public string? BuildResult { get; set; }
    public List<ComboBoxItem<string, string>> ComponentList { get; } = [];
    public bool IsLoading { get; set; }

    public void SelectFirstComponent()
        => Component = ComponentList.FirstOrDefault()?.Value;
}
