namespace SpeedGameApp.Business.Services.Models;

using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Class which represents a question theme.
/// </summary>
public sealed class QcmThemeDto
{
    /// <summary>
    ///     Gets or sets the theme id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the thme name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Method which map <see cref="QcmThemeDto" /> from <see cref="QcmTheme" />.
    /// </summary>
    /// <param name="questionTheme">The theme to map.</param>
    /// <returns>Returns the <see cref="QcmThemeDto" />.</returns>
    public static QcmThemeDto FromQcmTheme(QcmTheme questionTheme) => new()
    {
        Id = questionTheme.Id,
        Name = questionTheme.Name,
    };
}
