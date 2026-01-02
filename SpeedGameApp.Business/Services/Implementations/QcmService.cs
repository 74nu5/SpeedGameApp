namespace SpeedGameApp.Business.Services.Implementations;

using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.Interfaces;

/// <summary>
///     Service responsible for QCM (multiple choice questions) management.
/// </summary>
public sealed class QcmService(
    IQuestionAccessLayer questionAccessLayer,
    IPartyStateManager stateManager) : IQcmService
{
    /// <inheritdoc />
    public void SetRandomQcm(Guid partyId)
    {
        var question = questionAccessLayer.GetRandom();
        stateManager.SetCurrentQcm(partyId, QcmQuestionDto.FromQcmQuestion(question));
    }

    /// <inheritdoc />
    public void PropositionQcmTeam(Guid partyId, Guid teamId, string response)
        => stateManager.PropositionQcmTeam(partyId, teamId, response);
}
