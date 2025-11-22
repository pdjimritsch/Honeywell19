using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace MediaPlayer.Models;

[ImmutableObject(true)] public sealed partial class PrivacyViewModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public string Title { get; set; } = "Privacy Policy";

    #endregion
}
