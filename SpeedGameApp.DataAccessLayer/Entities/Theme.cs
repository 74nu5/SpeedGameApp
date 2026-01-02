namespace SpeedGameApp.DataAccessLayer.Entities;

using System.ComponentModel.DataAnnotations;

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
    [MaxLength(250)]
    public required string Name { get; set; }
}
