using System.ComponentModel.DataAnnotations;

namespace Ping.Client.Counter.Models;

internal sealed class CounterSettings
{
    [Required]
    [StringLength(30, ErrorMessage = "Name is too long.")]
    public string? ComponentTypeName { get; set; }
    public string? Namespace { get; set; }
    public bool ThrowErrorOnSave { get; set; }
}
