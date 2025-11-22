using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.Data.Factory;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace MediaPlayer.Models;

[ImmutableObject(true)] public sealed partial class HomeViewModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public List<IFormFile> UploadedVideos { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public IHostSettings? Settings { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public Visitor? Visitor { get; set; } = null!;

    #endregion
}
