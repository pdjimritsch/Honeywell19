using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.Data.Factory;
using MediaPlayer.Helpers;
using MediaPlayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MediaPlayer.Controllers
{
    public class PostCaptureController : Controller
    {
        #region Members

        /// <summary>
        /// 
        /// </summary>
        public const string ControllerName = "PostCapture";


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
        public PostCaptureController(
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

        #region PostCapture View

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet] public IActionResult Index()
        {
            Visitor = CurrentVisitor.Get(HttpContext);

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost] public IActionResult Post()
        {
            PostCaptureViewModel vm = new();

            var sequence = HttpContext.Session.Get(RouteParameters.Key);

            var parameters = RouteParameters.Parse(Encoding.UTF8.GetString(sequence ?? []));

            if (parameters != null)
            {
                vm.Selection = [];

                vm.Selection.AddRange(parameters.Movies);
            }

            if (vm.Selection.Count > 0)
            {
                string title = string.Empty;

                if (Request.Form.ContainsKey("movie-preview"))
                {
                    title = Request.Form["movie-preview"].ToString();

                    var preview = vm.Selection.FirstOrDefault(mv => mv.Title == title);

                    if (preview != null)
                    {
                        var bytes = Encoding.UTF8.GetBytes(preview.ToString().ToCharArray());

                        HttpContext.Session.Set(Movie.Key, bytes);
                    }
                }
            }

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToActionPermanent("Index", RecentMovieController.ControllerName);
        }

        #endregion
    }
}
