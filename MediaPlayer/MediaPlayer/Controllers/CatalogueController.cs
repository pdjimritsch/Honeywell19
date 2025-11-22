using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.Data.Factory;
using MediaPlayer.Helpers;
using MediaPlayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MediaPlayer.Controllers;

public class CatalogueController : Controller
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    public const string ControllerName = "Catalogue";

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
    /// <param name="environment"></param>
    /// <param name="contextAccessor"></param>
    /// <param name="settings"></param>
    public CatalogueController(
        IHostEnvironment? environment, IHttpContextAccessor contextAccessor, IHostSettings? settings)
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

    #region Catalogue View

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Index()
    {
        return View(CatalogueViewModel.Get());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Index([FromBody] CatalogueViewModel vm)
    {
        Visitor = CurrentVisitor.Get(HttpContext);

        if (Visitor == null)
        {
            Visitor = new Visitor { IsContentAppearing = true };
        }
        else
        {
            Visitor.IsContentAppearing = true;
        }

        string? preview = Request.Form["preview"].ToString();

        if (preview == null)
        {
            // The visitor request to watch the selected movie was aborted due to technical difficulties

            return RedirectToActionPermanent("Index", HomeController.ControllerName);
        }

        if (vm == null)
        {
            vm = CatalogueViewModel.Get();
        }

        var movie = vm.Movies.FirstOrDefault(mv => mv.Title == preview);

        if (movie == null)
        {
            // The visitor request to watch the selected movie was aborted due to technical difficulties

            return RedirectToActionPermanent("Index", HomeController.ControllerName);
        }

        var video = new Video
        {
            ContentDirectory = AppGenerator.ContentDirectory,
            ContentType = movie.ContentType,
            ContentLength = movie.ContentLength,
            FileExtension = movie.fileExtension,
            FileName = movie.FileName,
            Title = movie.Title,
        };

        HttpContext.Session.Set(nameof(Movie), Encoding.UTF8.GetBytes(new Movie(video).ToString().ToCharArray()));

        if (Visitor != null)
        {
            Visitor.VideoContentLength = movie.ContentLength;

            Visitor.VideoContentType = movie.ContentType;

            Visitor.VideoFileName = movie.FileName;

            Visitor.VideoTitle = movie.Title;

            Visitor.IsContentAppearing = true;
        }

        Visitor = CurrentVisitor.Set(HttpContext, null, Visitor);

        return RedirectToActionPermanent("Index", RecentMovieController.ControllerName);
    }

    #endregion

}
