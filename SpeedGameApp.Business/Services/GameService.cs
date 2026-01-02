namespace SpeedGameApp.Business.Services;

using SpeedGameApp.Business.Common;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataEnum;

/// <summary>
///     Facade service that delegates to specialized services for backward compatibility.
///     This service will be deprecated in favor of using specialized services directly.
/// </summary>
[Obsolete("Use specialized services (IPartyManagementService, IQcmService, IGameplayService, IThemeService) instead. This facade is kept for backward compatibility.")]
public sealed class GameService(
    IPartyManagementService partyManagement,
    IQcmService qcmService,
    IGameplayService gameplayService,
    IThemeService themeService)
{
    // Delegation to PartyManagementService
    public event EventHandler<PartyDto>? PartyChanged
    {
        add => partyManagement.PartyChanged += value;
        remove => partyManagement.PartyChanged -= value;
    }

    public IReadOnlyDictionary<Guid, PartyDto> Parties
        => partyManagement.Parties;

    // PartyManagementService delegation
    public Task<Result<Guid>> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
        => partyManagement.CreatePartyAsync(partyName, cancellationToken);

    public Task<Result<Guid>> CreateTeamPartyAsync(Guid partyId, string teamName, CancellationToken cancellationToken)
        => partyManagement.CreateTeamPartyAsync(partyId, teamName, cancellationToken);

    public Task<PartyDto?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
        => partyManagement.GetPartyAsync(id, cancellationToken);

    public Task<Dictionary<Guid, PartyDto>> GetDbPartiesAsync(CancellationToken cancellationToken)
        => partyManagement.GetDbPartiesAsync(cancellationToken);

    public Task LoadPartyAsync(Guid id, CancellationToken cancellationToken)
        => partyManagement.LoadPartyAsync(id, cancellationToken);

    public Task SavePartyAsync(Guid id, CancellationToken cancellationToken)
        => partyManagement.SavePartyAsync(id, cancellationToken);

    public void DeleteParty(Guid id)
        => partyManagement.DeleteParty(id);

    public void DeleteAllParties()
        => partyManagement.DeleteAllParties();

    public Task DeleteDbPartyAsync(Guid id, CancellationToken cancellationToken)
        => partyManagement.DeleteDbPartyAsync(id, cancellationToken);

    public Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken)
        => partyManagement.DeleteTeamAsync(partyId, teamId, cancellationToken);

    // QcmService delegation
    public void SetRandomQcm(Guid partyId)
        => qcmService.SetRandomQcm(partyId);

    public void PropositionQcmTeam(Guid partyId, Guid teamId, string response)
        => qcmService.PropositionQcmTeam(partyId, teamId, response);

    // GameplayService delegation
    public Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken)
        => gameplayService.AddPointsAsync(teamDto, points, cancellationToken);

    public void BuzzTeam(Guid partyId, Guid teamId)
        => gameplayService.BuzzTeam(partyId, teamId);

    public void PropositionTeam(Guid partyId, Guid teamId, string response)
        => gameplayService.PropositionTeam(partyId, teamId, response);

    public void ResetTeam(Guid partyId)
        => gameplayService.ResetTeam(partyId);

    public void SetCurrentResponse(Guid partyId, ResponseType responseType)
        => gameplayService.SetCurrentResponse(partyId, responseType);

    // ThemeService delegation
    public Task<IEnumerable<ThemeDto>> GetThemesAsync(Guid partyId)
        => themeService.GetThemesAsync(partyId);

    public void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme)
        => themeService.SelectTheme(partyId, teamId, theme);

    public void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme)
        => themeService.ChoiceTheme(partyId, teamId, theme);

    public void ResetThemesChoices(Guid partyId)
        => themeService.ResetThemesChoices(partyId);

    public void HideTheme(Guid partyId)
        => themeService.HideTheme(partyId);

    public void ShowTheme(Guid partyId)
        => themeService.ShowTheme(partyId);

    public void GenerateThemes(Guid partyId)
        => themeService.GenerateThemes(partyId);
}
