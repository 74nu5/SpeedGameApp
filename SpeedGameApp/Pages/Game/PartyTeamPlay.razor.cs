namespace SpeedGameApp.Pages.Game;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.Shared.Components.Responses;

/// <summary>
///     The party team play.
/// </summary>
public sealed partial class PartyTeamPlay : PartyPageBase
{
    private Proposition? proposition;

    private QCM? qcm;

    private IEnumerable<ThemeDto> themes = new List<ThemeDto>();

    /// <inheritdoc />
    public PartyTeamPlay(IPartyManagementService partyManagementService, IQcmService qcmService, IGameplayService gameplayService, IThemeService themeService, NavigationManager navigationManager)
            : base(partyManagementService, qcmService, gameplayService, themeService, navigationManager)
    {
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        this.CurrentParty.PartyReset += this.CurrentPartyOnPartyResetAsync;
        this.themes = this.CurrentParty.RandomThemes;
    }

    private async Task SelectThemeAsync(ThemeDto theme)
    {
        this.ThemeService.SelectTheme(this.PartyId, this.TeamId, theme);
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }

    private string GetHeaderClasses()
    {
        if (this.CurrentTeam.Buzz)
        {
            return "bg-gradient-to-r from-green-500 to-green-600 text-white";
        }

        return "bg-gradient-to-r from-3b-blue to-3b-blue-dark text-white";
    }

    private string GetLeaderboardItemClasses(TeamDto team)
    {
        if (team.Id == this.TeamId)
        {
            return "bg-3b-yellow/10 border-l-4 border-3b-yellow";
        }

        if (team.Buzz)
        {
            return "bg-green-50 border-l-4 border-green-500";
        }

        return "hover:bg-gray-50";
    }

    private string GetPositionBadgeClasses(int position)
        => position switch
        {
            1 => "bg-yellow-400 text-yellow-900",
            2 => "bg-gray-300 text-gray-800",
            3 => "bg-orange-400 text-orange-900",
            _ => "bg-gray-200 text-gray-700",
        };

    private string GetTeamNameClasses(TeamDto team)
    {
        if (team.Id == this.TeamId)
        {
            return "font-bold text-3b-blue";
        }

        return "font-semibold text-gray-800";
    }

    private string GetScoreClasses(TeamDto team)
    {
        if (team.Score < 0)
        {
            return "text-red-600";
        }

        if (team.Score > 0)
        {
            return "text-green-600";
        }

        return "text-gray-700";
    }

    private string GetThemeCardClasses(ThemeDto themeDto)
        => (this.TeamId, themeDto.AlreadyTaken, themeDto.Team?.Id) switch
        {
            (null, _, _) => "bg-gray-100 text-gray-700 border-2 border-gray-300",
            (_, true, var teamId) when teamId == this.TeamId => "bg-gradient-to-br from-green-500 to-green-600 text-white border-2 border-green-700 shadow-medium",
            (_, true, { } teamId) when this.CurrentParty.Teams.ContainsKey(teamId) => "bg-gradient-to-br from-red-500 to-red-600 text-white border-2 border-red-700 opacity-75",
            (_, true, null) => "bg-gray-400 text-gray-800 border-2 border-gray-500",
            _ => "bg-white text-gray-700 border-2 border-3b-blue hover:bg-3b-blue hover:text-white",
        };

    private void BuzzTeam()
        => this.GameplayService.BuzzTeam(this.PartyId, this.CurrentTeam.Id);

    private void Callback(string response)
        => this.GameplayService.PropositionTeam(this.PartyId, this.CurrentTeam.Id, response);

    private void CallbackQcm(string response)
        => this.QcmService.PropositionQcmTeam(this.PartyId, this.CurrentTeam.Id, response);

    private async void CurrentPartyOnPartyResetAsync(object? sender, EventArgs e)
    {
        if (this.proposition is not null)
            await this.proposition.ResetAsync().ConfigureAwait(true);

        if (this.qcm is not null)
            await this.qcm.ResetAsync().ConfigureAwait(true);
    }
}
