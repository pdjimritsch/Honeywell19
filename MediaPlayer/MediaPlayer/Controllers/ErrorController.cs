using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MediaPlayer.Controllers
{
    public class ErrorController : Controller
    {
        #region Members

        /// <summary>
        /// 
        /// </summary>
        public const string ControllerName = "Error";

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
        public ErrorController(
            IHostEnvironment? environment, IHttpContextAccessor? contextAccessor, IHostSettings? settings)
        {
            _environment = environment;

            _contextAccessor = contextAccessor;

            _settings = settings;
        }

        #endregion

        #region Error Controller View

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Index()
        {
            ErrorViewModel vm = new();

            vm.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            return View(vm);
        }

        #endregion

    }
}
