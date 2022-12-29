namespace SpeedGameApp.DataAccessLayer.Entities;

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
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the theme questions.
    /// </summary>
    public ICollection<QcmQuestion> Questions { get; set; } = new HashSet<QcmQuestion>();
}
