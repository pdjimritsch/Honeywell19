using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.Data.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaPlayer.Controllers;

using Helpers;
using Models;
using System.Text;

[AllowAnonymous] public class HomeController : Controller
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    public const string ControllerName = "Home";

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
    public HomeController(
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
    [BindProperty] public List<IFormFile> UploadedVideos { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public Visitor? Visitor { get; set; }

    #endregion

    #region Actions

    #region Index View

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet] public IActionResult Index()
    {
        HomeViewModel vm = new()
        {
            Settings = _settings,
            UploadedVideos = [],
            Visitor = Visitor,
        };

        if (_environment != null && string.IsNullOrEmpty(AppGenerator.ContentDirectory))
        {
            AppGenerator.ContentDirectory = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        Visitor = CurrentVisitor.Get(HttpContext);

        if (Request.Headers.ContainsKey("Referer"))
        {
            var previousPage = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(previousPage) && previousPage.Contains("RecentMovie"))
            {
                // reset the form for the visitor to continue with movie upload 

                if (Visitor != null)
                {
                    Visitor.IsContentAppearing = false;

                    Visitor.MessageIndex = 0;
                }
            }
            else
            {
                // display the controls for the visitor to upload the videos.

                if (Visitor != null)
                {
                    Visitor.IsContentAppearing = true;

                    Visitor.MessageIndex = 1;
                }
            }
        }
        else if (Visitor != null)
        {
            Visitor.IsContentAppearing = !Visitor.IsContentAppearing;

            if (Visitor.IsContentAppearing)
            {
                // display the controls for the visitor to upload the videos.

                Visitor.IsContentAppearing = !Visitor.IsContentAppearing;

                Visitor.MessageIndex = 1;
            }
            else
            {
                // reset the form for the visitor to continue with movie upload 

                Visitor.MessageIndex = 0;
            }
        }

        vm.Visitor = Visitor = CurrentVisitor.Set(HttpContext, null, Visitor);

        return View(vm);
    }

    #endregion

    #region Posted Upload

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost] public async Task<IActionResult> UploadAsync()
    {
        if (!ModelState.IsValid)
        {
            if (Visitor != null)
            {
                Visitor.MessageIndex = 0;
                Visitor.IsContentAppearing = false;
                Visitor = CurrentVisitor.Set(HttpContext, null, Visitor);
            }
            return await Task.FromResult(RedirectToActionPermanent("Index"));
        }

        if ((Request.ContentLength == 0) || (Request.Body == null))
        {
            if (Visitor != null)
            {
                Visitor.MessageIndex = 0;
                Visitor.IsContentAppearing = false;
                Visitor = CurrentVisitor.Set(HttpContext, null, Visitor);
            }
            return await Task.FromResult(RedirectToActionPermanent("Index"));
        }

        if (UploadedVideos.Count == 0)
        {
            if (Visitor != null)
            {
                Visitor.MessageIndex = 0;
                Visitor.IsContentAppearing = false;
                Visitor = CurrentVisitor.Set(HttpContext, null, Visitor);
            }
            return await Task.FromResult(RedirectToActionPermanent("Index"));
        }

        var approved = UploadedVideos.Any(v => v.Length <= (_settings?.MaxMovieSize ?? 0));

        if (!approved)
        {
            if (Visitor != null)
            {
                Visitor.MessageIndex = 0;
                Visitor.IsContentAppearing = false;
                Visitor = CurrentVisitor.Set(HttpContext, null, Visitor);
            }
            return await Task.FromResult(RedirectToActionPermanent("Index"));
        }

        Visitor = SetVisitor(HttpContext, UploadedVideos, Visitor);

        List<Movie> movies = GetPostedMovies(UploadedVideos, Visitor, _settings);

        var count = movies.Count;

        foreach (var movie in movies)
        {
            if (!string.IsNullOrEmpty(movie.FileName))
            {
                TokenStore.Instance.Add(movie.FileName, movie);
            }

            if (count == 1)
            {
                // Save the visitor's reference to the selected video

                HttpContext.Session.Set(nameof(Movie), Encoding.UTF8.GetBytes(movie.ToString().ToCharArray()));

                if (Visitor != null)
                {
                    Visitor.IsContentAppearing = true;
                }

                return await Task.FromResult(RedirectToActionPermanent("Index", RecentMovieController.ControllerName));
            }
        }

        if (count > 0)
        {
            if (Visitor != null)
            {
                Visitor.IsContentAppearing = true;
            }

            RouteParameters parameters = new();

            parameters.Movies.AddRange(movies);

            HttpContext.Session.Remove(nameof(Movie)); // Multiple movies have been selected

            HttpContext.Session.Set(RouteParameters.Key, Encoding.UTF8.GetBytes(parameters.ToString().ToCharArray()));

            return await Task.FromResult(RedirectToActionPermanent("Index", RecentMovieController.ControllerName));
        }

        if (Visitor != null)
        {
            Visitor.MessageIndex = 0;

            Visitor.IsContentAppearing = true;
        }

        Visitor = CurrentVisitor.Set(HttpContext, null, Visitor);

        return await Task.FromResult(RedirectToActionPermanent("Index"));
    }

    #endregion

    #region Privacy View

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet] public IActionResult Privacy()
    {
        return View(new PrivacyViewModel());
    }

    #endregion

    #endregion

    #region Functions

    /// Acquires the representative movie from the posted uploaded movie.
    /// </summary>
    /// <param name="video"></param>
    /// <param name="visitor"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    private static Movie? GetPostedMovie(IFormFile? video, Visitor? visitor, string? filename)
    {
        Movie? movie = default;

        if (video != null && !string.IsNullOrEmpty(filename) && System.IO.File.Exists(filename))
        {
            movie = TokenStore.Instance.Get(filename).Cast<Movie>().FirstOrDefault();

            var reference = new FileInfo(video.FileName);

            if (movie == null)
            {
                movie = new Movie();

                Video presentation = new()
                {
                    ContentDirectory = AppGenerator.ContentDirectory,
                    ContentLength = video.Length,
                    ContentType = video.ContentType,
                    FileExtension = reference.Extension,
                    FileName = video.FileName,
                    Title = reference.Name,
                };

                movie.AddOrRemoveMovie(visitor, presentation, filename);
            }
            else
            {
                var visitors = movie.Visitors;

                if (visitors == null)
                {
                    // Save the visitor's reference to the selected video

                    Video presentation = new()
                    {
                        ContentDirectory = AppGenerator.ContentDirectory,
                        ContentLength = video.Length,
                        ContentType = video.ContentType,
                        FileExtension = reference.Extension,
                        FileName = video.FileName,
                        Title = reference.Name,
                    };


                    movie.AddOrRemoveMovie(visitor, presentation, filename);
                }
                else
                {
                    var count = visitors.Count(v => v.Equals(visitor));

                    if (count == 0)
                    {
                        // Save the visitor's reference to the selected video

                        Video presentation = new()
                        {
                            ContentDirectory = AppGenerator.ContentDirectory,
                            ContentLength = video.Length,
                            ContentType = video.ContentType,
                            FileExtension = reference.Extension,
                            FileName = video.FileName,
                            Title = reference.Name,
                        };

                        movie.AddOrRemoveMovie(visitor, presentation, filename);
                    }
                }
            }
        }

        return movie;
    }

    /// <summary>
    /// Acquires the selected movies from the upload activity.
    /// </summary>
    /// <param name="videos"></param>
    /// <param name="visitor"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    private static List<Movie> GetPostedMovies(IEnumerable<IFormFile> videos, Visitor? visitor, IHostSettings? settings)
    {
        List<Movie> sequence = [];

        foreach (var video in videos)
        {
            if (video.Length > (settings?.MaxMovieSize ?? 0))
            {
                // Large sized videos will be ignored
                continue;
            }

            // Save the uploaded video within the web server file physical storage
            var filename = AppGenerator.SaveVideo(video);

            // Get the representative movie from the filename
            var movie = GetPostedMovie(video, visitor, filename);

            if (movie != null)
            {
                sequence.Add(movie);
            }
        }

        return sequence;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="resources"></param>
    /// <param name="visitor"></param>
    /// <returns></returns>
    private static Visitor? SetVisitor(HttpContext context, List<IFormFile> resources, Visitor? visitor)
    {
        visitor = CurrentVisitor.Get(context);

        if ((visitor == null) && (resources.Count == 1))
        {
            // Assign the selected video for the current visitor
            visitor = CurrentVisitor.Set(context, resources[0]);
        }
        else if ((visitor == null) && (resources.Count > 1))
        {
            // Create the visitor from the current session context
            visitor = CurrentVisitor.Set(context, null);
        }
        else if ((visitor != null) && (resources.Count == 1))
        {
            // Assign the selected video for the current visitor
            visitor = CurrentVisitor.Set(context, resources[0], visitor);
        }

        return visitor;
    }

    #endregion
}
