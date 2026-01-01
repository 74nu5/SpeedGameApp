namespace SpeedGameApp.DataAccessLayer.AccessLayers;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;

public sealed class QuestionAccessLayer(AppContext context)
{
    public QcmQuestion GetRandom()
    {
        var totalQuestion = context.Questions.Count();
        var r = Random.Shared.Next(1, totalQuestion); // .NET 6+ Random.Shared thread-safe

        return context.Questions.Include(q => q.Theme).Skip(r).Take(1).FirstOrDefault() ?? throw new("No question");
    }
}
