namespace SpeedGameApp.DataAccessLayer.AccessLayers;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Class which represents the theme access layer.
/// </summary>
public sealed class ThemeAccessLayer(AppContext context)
{
    /// <summary>
    ///     Gets all themes.
    /// </summary>
    /// <returns>The list of themes.</returns>
    public async Task<List<Theme>> GetAllThemesAsync()
        => await context.Themes.ToListAsync();
}
