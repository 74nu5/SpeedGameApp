namespace SpeedGameApp.DataAccessLayer.AccessLayers;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;

public sealed class QuestionAccessLayer
{
    private readonly AppContext context;

    private static readonly Random Random = new();

    public QuestionAccessLayer(AppContext context) => this.context = context;

    public QcmQuestion GetRandom()
    {
        var totalQuestion = this.context.Questions.Count();
        var r = Random.Next(1, totalQuestion);

        return this.context.Questions.Include(q => q.Theme).Skip(r).Take(1).FirstOrDefault() ?? throw new("No question");
    }
}
