namespace SpeedGameApp.Business.Services.Implementations;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.DataEnum;

/// <summary>
///     Service responsible for real-time gameplay mechanics (scoring, buzzing, responses).
/// </summary>
public sealed class GameplayService(
    IPartyStateManager stateManager,
    IPartyManagementService partyManagementService) : IGameplayService
{
    /// <inheritdoc />
    public async Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken)
    {
        stateManager.AddPoints(teamDto, points);
        await partyManagementService.SavePartyAsync(teamDto.PartyId, cancellationToken);
    }

    /// <inheritdoc />
    public void BuzzTeam(Guid partyId, Guid teamId)
        => stateManager.BuzzTeam(partyId, teamId);

    /// <inheritdoc />
    public void PropositionTeam(Guid partyId, Guid teamId, string response)
        => stateManager.PropositionTeam(partyId, teamId, response);

    /// <inheritdoc />
    public void ResetTeam(Guid partyId)
        => stateManager.ResetTeam(partyId);

    /// <inheritdoc />
    public void SetCurrentResponse(Guid partyId, ResponseType responseType)
        => stateManager.SetCurrentResponse(partyId, responseType);
}
