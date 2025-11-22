using Microsoft.AspNetCore.Mvc;
using MediaPlayer.Data.Factory;
using System.ComponentModel;

namespace MediaPlayer.Models;

/// <summary>
/// 
/// </summary>
[ImmutableObject(true)] public sealed partial class PostCaptureViewModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public List<Movie> Selection { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public Visitor? Visitor { get; set; } = default;

    #endregion
}
