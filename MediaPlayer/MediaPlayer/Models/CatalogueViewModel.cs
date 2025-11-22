using MediaPlayer.Data.Factory;
using MediaPlayer.Data.Factory.Abstraction;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace MediaPlayer.Models;

[ImmutableObject(true)] public sealed partial class CatalogueViewModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public List<WatchedMovieViewModel> Movies { get; set; } = [];

    #endregion

    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static CatalogueViewModel Get()
    {
        var vm = new CatalogueViewModel();

        var collection = TokenStore.Instance.Get();

        foreach (var entry in collection)
        {
            var filename = entry.Key;

            var reference = new FileInfo(filename);

            var name = reference.Name;

            var movie = entry.Value.Cast<Movie>().FirstOrDefault();

            if (!string.IsNullOrEmpty(filename) && (movie != null))
            {
                foreach (var visitor in movie.Visitors ?? Enumerable.Empty<IVisitor>())
                {
                    vm.Movies.Add(new WatchedMovieViewModel
                    {
                        ContentDirectory = AppGenerator.ContentDirectory ?? string.Empty,
                        ContentLength = movie.ContentLength,
                        ContentType = movie.ContentType ?? string.Empty,
                        fileExtension = reference.Extension,
                        FileName = filename ?? string.Empty,
                        Title = name ?? string.Empty,
                        Visitor = visitor
                    });
                }
            }
        }

        return vm;
    }

    #endregion
}
