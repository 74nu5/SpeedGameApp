namespace SpeedGameApp.DataAccessLayer.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Class which represents a question theme.
/// </summary>
public sealed class QcmTheme
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

    /// <summary>
    ///     Gets or sets the theme questions.
    /// </summary>
    public ICollection<QcmQuestion> Questions { get; set; } = new HashSet<QcmQuestion>();
}
