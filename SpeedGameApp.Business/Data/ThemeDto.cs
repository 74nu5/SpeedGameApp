namespace SpeedGameApp.Business.Data;

/// <summary>
///     Class which represents the theme dto.
/// </summary>
public sealed record ThemeDto
{
    /// <summary>
    ///     Gets or sets the theme id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the theme name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the theme team.
    /// </summary>
    public TeamDto? Team { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the theme is already taken.
    /// </summary>
    public bool AlreadyTaken { get; set; }
}
