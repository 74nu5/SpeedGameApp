namespace SpeedGameApp.Business.Services;

using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;

public sealed class CsvService
{
    private readonly AppContext context;

    public CsvService(AppContext context) => this.context = context;

    public async Task InsertQuestionsAsync(List<QcmQuestionDto> questions)
    {
        foreach (var themeDto in questions.Select(q => q.Theme).DistinctBy(t => t.Name))
        {
            var themeFound = this.context.Themes.FirstOrDefault(t => t.Name == themeDto.Name);

            if (themeFound == null)
            {
                _ = this.context.Themes.Add(new()
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

                var themeFound = this.context.Themes.FirstOrDefault(t => t.Name == question.Theme.Name);

                if (themeFound is not null)
                    newQuestion.Theme = themeFound;

                _ = this.context.Questions.Add(newQuestion);
            }

            _ = await this.context.SaveChangesAsync();
        }
    }

    public IEnumerable<QcmQuestionDto> CsvToQuestions(IEnumerable<string> lines) => lines.Select(ProcessLine);

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
