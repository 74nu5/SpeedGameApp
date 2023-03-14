namespace SpeedGameApp.DataAccessLayer.AccessLayers;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Class which represents the theme access layer.
/// </summary>
public sealed class ThemeAccessLayer
{
    private readonly AppContext context;

    /// <summary>
    ///    Initializes a new instance of the <see cref="ThemeAccessLayer"/> class.
    /// </summary>
    /// <param name="context">The application context.</param>
    public ThemeAccessLayer(AppContext context)
        => this.context = context;

    /// <summary>
    ///     Gets all themes.
    /// </summary>
    /// <returns>The list of themes.</returns>
    public async Task<List<Theme>> GetAllThemesAsync()
        => await this.context.Themes.ToListAsync();
}
