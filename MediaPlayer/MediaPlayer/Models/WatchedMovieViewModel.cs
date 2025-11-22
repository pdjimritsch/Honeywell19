using System.ComponentModel;

using MediaPlayer.Data.Factory.Abstraction;

using Microsoft.AspNetCore.Mvc;

namespace MediaPlayer.Models;

[ImmutableObject(true)] public sealed partial class WatchedMovieViewModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public string ContentDirectory { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    [BindProperty]  public long ContentLength { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public string ContentType { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public string fileExtension { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public string FileName { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public string Title { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public IVisitor? Visitor { get; set; } = null!;

    #endregion
}
