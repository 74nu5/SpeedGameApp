namespace SpeedGameApp.Pages;

using SpeedGameApp.Business.Data;
using SpeedGameApp.DataEnum;

public sealed partial class PartyAdmin : PartyPageBase
{
    private async Task AddPointsAsync(TeamDto teamDto, int points)
    {
        this.CancellationTokenSource = new();
        await this.GameService.AddPointsAsync(teamDto, points, this.CancellationTokenSource.Token);
    }

    private void SetResponse(ResponseType responseType)
        => this.GameService.SetCurrentResponse(this.PartyId, responseType);

    private async Task DeleteTeamAsync(Guid partyId, Guid teamId)
    {
        this.CancellationTokenSource = new();
        await this.GameService.DeleteTeamAsync(partyId, teamId, this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }

    private void RandomizeQcm()
        => this.GameService.SetRandomQcm(this.PartyId);

    private void ResetQuestion()
        => this.GameService.ResetTeam(this.PartyId);
}
