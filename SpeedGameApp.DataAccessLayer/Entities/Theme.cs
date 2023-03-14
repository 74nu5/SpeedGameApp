namespace SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Class which represent a theme.
/// </summary>
public sealed class Theme
{
    /// <summary>
    ///     Gets or sets the theme id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the thme name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
