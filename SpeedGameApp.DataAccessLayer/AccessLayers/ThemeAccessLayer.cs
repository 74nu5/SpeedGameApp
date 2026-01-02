namespace SpeedGameApp.DataAccessLayer.AccessLayers;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataAccessLayer.Interfaces;

/// <summary>
///     Data access layer for theme operations.
/// </summary>
public sealed class ThemeAccessLayer(SpeedGameDbContext context) : IThemeAccessLayer
{
    /// <inheritdoc/>
    public async Task<List<Theme>> GetAllThemesAsync()
        => await context.Themes.ToListAsync();
}
