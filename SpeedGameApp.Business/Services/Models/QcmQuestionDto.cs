namespace SpeedGameApp.Business.Services.Models;

using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;

/// <summary>
///     Class which represent a qcm question.
/// </summary>
public sealed class QcmQuestionDto
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
    ///     Gets or sets the question theme.
    /// </summary>
    public QcmThemeDto Theme { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the question.
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question option 1.
    /// </summary>
    public string Option1 { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question option 2.
    /// </summary>
    public string Option2 { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question option 3.
    /// </summary>
    public string Option3 { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question option 4.
    /// </summary>
    public string Option4 { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the question response.
    /// </summary>
    public string Response { get; set; } = string.Empty;

    /// <summary>
    ///     Method which map <see cref="QcmQuestionDto" /> from <see cref="QcmQuestion" />.
    /// </summary>
    /// <param name="question">The question to map.</param>
    /// <returns>Returns the <see cref="QcmQuestionDto" />.</returns>
    public static QcmQuestionDto FromQcmQuestion(QcmQuestion question) => new()
    {
        Id = question.Id,
        Difficulty = question.Difficulty,
        Theme = QcmThemeDto.FromQcmTheme(question.Theme!),
        Question = question.Question,
        Option1 = question.Option1,
        Option2 = question.Option2,
        Option3 = question.Option3,
        Option4 = question.Option4,
        Response = question.Response,
    };
}
