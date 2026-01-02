namespace SpeedGameApp.Business.Services.Interfaces;

using SpeedGameApp.Business.Data;
using SpeedGameApp.DataEnum;

/// <summary>
///     Service responsible for real-time gameplay mechanics (scoring, buzzing, responses).
/// </summary>
public interface IGameplayService
{
    /// <summary>
    ///     Adds points to a team and saves to database.
    /// </summary>
    Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken);

    /// <summary>
    ///     Registers a team buzz.
    /// </summary>
    void BuzzTeam(Guid partyId, Guid teamId);

    /// <summary>
    ///     Submits a team's proposition/answer.
    /// </summary>
    void PropositionTeam(Guid partyId, Guid teamId, string response);

    /// <summary>
    ///     Resets team states for a party.
    /// </summary>
    void ResetTeam(Guid partyId);

    /// <summary>
    ///     Sets the current response type for a party.
    /// </summary>
    void SetCurrentResponse(Guid partyId, ResponseType responseType);
}
