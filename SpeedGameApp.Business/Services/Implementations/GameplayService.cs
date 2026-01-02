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
    private readonly IPartyStateManager stateManager = stateManager;
    private readonly IPartyManagementService partyManagementService = partyManagementService;

    /// <inheritdoc />
    public async Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken)
    {
        this.stateManager.AddPoints(teamDto, points);
        await this.partyManagementService.SavePartyAsync(teamDto.PartyId, cancellationToken);
    }

    /// <inheritdoc />
    public void BuzzTeam(Guid partyId, Guid teamId)
        => this.stateManager.BuzzTeam(partyId, teamId);

    /// <inheritdoc />
    public void PropositionTeam(Guid partyId, Guid teamId, string response)
        => this.stateManager.PropositionTeam(partyId, teamId, response);

    /// <inheritdoc />
    public void ResetTeam(Guid partyId)
        => this.stateManager.ResetTeam(partyId);

    /// <inheritdoc />
    public void SetCurrentResponse(Guid partyId, ResponseType responseType)
        => this.stateManager.SetCurrentResponse(partyId, responseType);
}
