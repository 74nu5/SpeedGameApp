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
    private readonly IQuestionAccessLayer questionAccessLayer = questionAccessLayer;
    private readonly IPartyStateManager stateManager = stateManager;

    /// <inheritdoc />
    public void SetRandomQcm(Guid partyId)
    {
        var question = this.questionAccessLayer.GetRandom();
        this.stateManager.SetCurrentQcm(partyId, QcmQuestionDto.FromQcmQuestion(question));
    }

    /// <inheritdoc />
    public void PropositionQcmTeam(Guid partyId, Guid teamId, string response)
        => this.stateManager.PropositionQcmTeam(partyId, teamId, response);
}
