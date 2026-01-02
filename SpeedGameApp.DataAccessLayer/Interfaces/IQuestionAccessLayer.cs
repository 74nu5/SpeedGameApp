namespace SpeedGameApp.DataAccessLayer.Interfaces;

using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Interface for question data access operations.
/// </summary>
public interface IQuestionAccessLayer
{
    /// <summary>
    ///     Gets a random QCM question.
    /// </summary>
    /// <returns>A random question.</returns>
    QcmQuestion GetRandom();
}
