namespace SpeedGameApp.Business.Services.Interfaces;

/// <summary>
///     Service responsible for QCM (multiple choice questions) management.
/// </summary>
public interface IQcmService
{
    /// <summary>
    ///     Sets a random QCM question for a party.
    /// </summary>
    void SetRandomQcm(Guid partyId);

    /// <summary>
    ///     Submits a team's answer to a QCM question.
    /// </summary>
    void PropositionQcmTeam(Guid partyId, Guid teamId, string response);
}
