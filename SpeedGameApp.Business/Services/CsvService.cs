namespace SpeedGameApp.Business.Services;

using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;

/// <summary>
///     Provides services for importing and processing QCM questions from CSV data.
/// </summary>
public sealed class CsvService
{
    private readonly SpeedGameDbContext context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CsvService" /> class.
    /// </summary>
    /// <param name="context">The database context to use for data operations.</param>
    public CsvService(SpeedGameDbContext context) => this.context = context;

    /// <summary>
    ///     Inserts a list of QCM questions into the database, adding new themes if necessary.
    /// </summary>
    /// <param name="questions">The list of questions to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InsertQuestionsAsync(List<QcmQuestionDto> questions)
    {
        foreach (var themeDto in questions.Select(q => q.Theme).DistinctBy(t => t.Name))
        {
            var themeFound = this.context.QcmThemes.FirstOrDefault(t => t.Name == themeDto.Name);

            if (themeFound == null)
            {
                _ = this.context.QcmThemes.Add(new()
                {
                    Name = themeDto.Name,
                });
            }
        }

        _ = await this.context.SaveChangesAsync();

        foreach (var question in questions)
        {
            var questionFound = this.context.Questions.FirstOrDefault(q => q.Question == question.Question);

            if (questionFound is null)
            {
                QcmQuestion newQuestion = new()
                {
                    Difficulty = question.Difficulty,
                    Question = question.Question,
                    Response = question.Response,
                    Option1 = question.Option1,
                    Option2 = question.Option2,
                    Option3 = question.Option3,
                    Option4 = question.Option4,
                };

                var themeFound = this.context.QcmThemes.FirstOrDefault(t => t.Name == question.Theme.Name);

                if (themeFound is not null)
                    newQuestion.Theme = themeFound;

                _ = this.context.Questions.Add(newQuestion);
            }

            _ = await this.context.SaveChangesAsync();
        }
    }

    /// <summary>
    ///     Converts a collection of CSV lines to a collection of <see cref="QcmQuestionDto" /> objects.
    /// </summary>
    /// <param name="lines">The CSV lines to convert.</param>
    /// <returns>An enumerable of <see cref="QcmQuestionDto" /> objects.</returns>
    public IEnumerable<QcmQuestionDto> CsvToQuestions(IEnumerable<string> lines) => lines.Select(ProcessLine);

    /// <summary>
    ///     Processes a single CSV line and converts it to a <see cref="QcmQuestionDto" /> object.
    /// </summary>
    /// <param name="line">The CSV line to process.</param>
    /// <returns>A <see cref="QcmQuestionDto" /> object representing the data in the line.</returns>
    private static QcmQuestionDto ProcessLine(string line)
    {
        var values = line.Split(',');

        var question = new QcmQuestionDto
        {
            Difficulty = Enum.Parse<Difficulty>(values[1]),
            Question = values[2],
            Option1 = values[3],
            Option2 = values[4],
            Option3 = values[5],
            Option4 = values[6],
            Response = values[7],
            Theme = new() { Name = values[0] },
        };

        return question;
    }
}
