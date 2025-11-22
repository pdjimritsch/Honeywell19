using MediaPlayer.Data.Factory;
using Microsoft.AspNetCore.Mvc;

namespace MediaPlayer.Models;

/// <summary>
/// 
/// </summary>
public partial class RecentMovieModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public Movie? Movie { get; set; } = default;

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public bool PlayVideo { get; set; } = false;

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
