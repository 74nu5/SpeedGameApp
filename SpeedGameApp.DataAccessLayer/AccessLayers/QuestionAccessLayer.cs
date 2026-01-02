namespace SpeedGameApp.DataAccessLayer.AccessLayers;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataAccessLayer.Interfaces;

/// <summary>
///     Data access layer for question operations.
/// </summary>
public sealed class QuestionAccessLayer(SpeedGameDbContext context) : IQuestionAccessLayer
{
    private readonly SpeedGameDbContext context = context;

    /// <inheritdoc/>
    public QcmQuestion GetRandom()
    {
        var totalQuestion = this.context.Questions.Count();
        var r = Random.Shared.Next(1, totalQuestion);

        return this.context.Questions.Include(q => q.Theme).Skip(r).Take(1).FirstOrDefault() ?? throw new InvalidOperationException("No question found");
    }
}
