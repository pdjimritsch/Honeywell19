using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.Data.Factory;
using MediaPlayer.Helpers;
using MediaPlayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MediaPlayer.Controllers;

/// <summary>
/// 
/// </summary>
public class RecentMovieController : Controller
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    public const string ControllerName = "RecentMovie";

    /// <summary>
    /// 
    /// </summary>
    private readonly IHostEnvironment? _environment;

    /// <summary>
    /// 
    /// </summary>
    private readonly IHttpContextAccessor? _contextAccessor;

    /// <summary>
    /// 
    /// </summary>
    private readonly IHostSettings? _settings;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public RecentMovieController(
        IHostEnvironment? environment, IHttpContextAccessor? contextAccessor, IHostSettings? settings)
    {
        _environment = environment;

        _contextAccessor = contextAccessor;

        _settings = settings;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public Visitor? Visitor { get; set; }

    #endregion

    #region RecentMovie View

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet] public IActionResult Index()
    {
        Visitor = CurrentVisitor.Get(HttpContext);

        RecentMovieModel vm = new();

        if (Request.Headers.ContainsKey("Referer"))
        {
            var previousPage = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(previousPage) && previousPage.Contains("Index"))
            {
                if (Visitor != null)
                {
                    Visitor.IsContentAppearing = true;
                }

            }
            else if (!string.IsNullOrEmpty(previousPage) && previousPage.Contains("Catalogue"))
            {
                if (Visitor != null)
                {
                    Visitor.IsContentAppearing = true;
                }
            }
        }

        vm.Movie = Movie.Parse(HttpContext.Session.GetString(nameof(Movie)));

        vm.PlayVideo = (vm.Movie != null);

        var sequence = HttpContext.Session.Get(RouteParameters.Key);

        var parameters = RouteParameters.Parse(Encoding.UTF8.GetString(sequence ?? []));

        if (parameters != null)
        {
            vm.Selection = parameters?.Movies ?? [];
        }

        if (Visitor != null)
        {
            Visitor.MessageIndex = 2;
        }

        Visitor = CurrentVisitor.Set(HttpContext, null, Visitor);

        vm.Visitor = Visitor;

        return View(vm);
    }

    #endregion
}
