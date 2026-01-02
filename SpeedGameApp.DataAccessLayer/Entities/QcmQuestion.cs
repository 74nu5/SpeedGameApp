namespace SpeedGameApp.DataAccessLayer.Entities;

using System.ComponentModel.DataAnnotations;

using SpeedGameApp.DataEnum;

/// <summary>
///     Class which represent a qcm question.
/// </summary>
public sealed class QcmQuestion
{
    /// <summary>
    ///     Gets or sets the id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the question difficulty.
    /// </summary>
    public Difficulty Difficulty { get; set; }

    /// <summary>
    ///     Gets or sets the question.
    /// </summary>
    [MaxLength(500)]
    public string Question { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question option 1.
    /// </summary>
    [MaxLength(500)]
    public string Option1 { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question option 2.
    /// </summary>
    [MaxLength(500)]
    public string Option2 { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question option 3.
    /// </summary>
    [MaxLength(500)]
    public string Option3 { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question option 4.
    /// </summary>
    [MaxLength(500)]
    public string Option4 { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question response.
    /// </summary>
    [MaxLength(500)]
    public string Response { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question theme id.
    /// </summary>
    public Guid ThemeId { get; set; }

    /// <summary>
    ///     Gets or sets the question theme.
    /// </summary>
    public QcmTheme? Theme { get; set; }
}
