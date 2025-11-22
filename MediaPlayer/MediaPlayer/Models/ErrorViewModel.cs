using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace MediaPlayer.Models;

[ImmutableObject(true)] public sealed partial class ErrorViewModel
{
    [BindProperty] public string? RequestId { get; set; } = null!;

    [BindProperty] public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
