namespace SpeedGameApp.Pages;

using SpeedGameApp.Business.Data;
using SpeedGameApp.DataEnum;

public sealed partial class PartyAdmin : PartyPageBase
{
    private async Task AddPointsAsync(TeamDto teamDto, int points)
    {
        this.CancellationTokenSource = new();
        await this.GameplayService.AddPointsAsync(teamDto, points, this.CancellationTokenSource.Token);
    }

    private void SetResponse(ResponseType responseType)
        => this.GameplayService.SetCurrentResponse(this.PartyId, responseType);

    private async Task DeleteTeamAsync(Guid partyId, Guid teamId)
    {
        this.CancellationTokenSource = new();
        await this.PartyManagementService.DeleteTeamAsync(partyId, teamId, this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }

    private void RandomizeQcm()
        => this.QcmService.SetRandomQcm(this.PartyId);

    private void ResetQuestion()
        => this.GameplayService.ResetTeam(this.PartyId);

    private void HideTheme()
        => this.ThemeService.HideTheme(this.PartyId);

    private void ShowTheme()
    {
        this.ThemeService.GenerateThemes(this.PartyId);
        this.ThemeService.ShowTheme(this.PartyId);
    }

    private string GetCardCss(ThemeDto themeDto)
        => (this.TeamId, themeDto.AlreadyTaken, themeDto.Team?.Id) switch {
            (null, _, _) => "card bg-light",
            (_, true, var teamId) when teamId == this.TeamId => "card bg-success",
            (_, true, { } teamId) when this.CurrentParty.Teams.ContainsKey(teamId) => "card bg-danger",
            (_, true, null) => "card bg-secondary",
            _ => "card bg-light",
        };
}
