namespace SpeedGameApp.DataAccessLayer.Interfaces;

using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Interface for theme data access operations.
/// </summary>
public interface IThemeAccessLayer
{
    /// <summary>
    ///     Gets all themes asynchronously.
    /// </summary>
    /// <returns>List of all themes.</returns>
    Task<List<Theme>> GetAllThemesAsync();
}
