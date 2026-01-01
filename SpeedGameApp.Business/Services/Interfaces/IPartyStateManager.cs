namespace SpeedGameApp.Business.Services.Interfaces;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataEnum;

/// <summary>
///     Interface for managing party game states.
/// </summary>
public interface IPartyStateManager
{
    /// <summary>
    ///     Adds points to a team.
    /// </summary>
    /// <param name="teamDto">The team.</param>
    /// <param name="points">The points to add.</param>
    void AddPoints(TeamDto teamDto, int points);

    /// <summary>
    ///     Sets the current response type for a party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="responseType">The response type.</param>
    void SetCurrentResponse(Guid partyId, ResponseType responseType);

    /// <summary>
    ///     Records a buzz from a team.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    void BuzzTeam(Guid partyId, Guid teamId);

    /// <summary>
    ///     Resets all teams in a party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    void ResetTeam(Guid partyId);

    /// <summary>
    ///     Records a proposition from a team.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    /// <param name="response">The team's response.</param>
    void PropositionTeam(Guid partyId, Guid teamId, string response);

    /// <summary>
    ///     Records a QCM proposition from a team.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    /// <param name="response">The team's response.</param>
    void PropositionQcmTeam(Guid partyId, Guid teamId, string response);

    /// <summary>
    ///     Sets the current QCM question for a party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="question">The QCM question.</param>
    void SetCurrentQcm(Guid partyId, QcmQuestionDto question);
}
